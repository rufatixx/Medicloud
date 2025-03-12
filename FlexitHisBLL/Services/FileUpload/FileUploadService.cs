
using FluentFTP;
using FluentFTP.Helpers;
using Medicloud.BLL.Services.ImageProcessing;
using Microsoft.Extensions.Configuration;

namespace Medicloud.BLL.Services.FileUpload
{
	public class FileUploadService:IFileUploadService
	{
		private readonly string _ftpPath;
		private readonly string _username;
		private readonly string _password;	
		private readonly IImageProcessingService _imageProcessingService;
		private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(5); // Max 5 concurrent connections

		public FileUploadService(IConfiguration configuration, IImageProcessingService imageProcessingService)
		{
			_ftpPath = configuration["FtpSettings:Url"];
			_username = configuration["FtpSettings:Username"];
			_password = configuration["FtpSettings:Password"];
			_imageProcessingService = imageProcessingService;
		}


		public async Task<bool> UploadFileAsync(byte[] fileBytes, string filePath,bool isImage=false)
		{
			if (isImage)
			{
				fileBytes=await _imageProcessingService.ResizeImageTo720pAsync(fileBytes);
			}
			filePath = $"/MedicloudV2/{filePath}";

			_semaphore.Wait();
			try
			{
				using (var client = new FtpClient(_ftpPath))
				{
					client.Credentials = new System.Net.NetworkCredential(_username, _password);
					client.Connect();

					string directoryPath = Path.GetDirectoryName(filePath);
					if (!client.DirectoryExists(directoryPath))
					{
						client.CreateDirectory(directoryPath);
						Console.WriteLine($"Directory created: {directoryPath}");
					}

					if (client.FileExists(filePath))
					{
						client.DeleteFile(filePath);
					}

					var result = client.UploadBytes(fileBytes, filePath);
					if (result.IsSuccess())
					{
						Console.WriteLine($"File uploaded successfully: {filePath}");
						return true;
					}
					else
					{
						Console.WriteLine($"File upload failed: {filePath}");

						return false;
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during file upload: {ex.Message}");

				return false;
			}
			finally
			{
				_semaphore.Release();
			}
		}


		public bool DeleteFile(string filePath)
		{
			filePath = $"/MedicloudV2/{filePath}";

			_semaphore.Wait();
			try
			{
				using (var client = new FtpClient(_ftpPath))
				{
					client.Credentials = new System.Net.NetworkCredential(_username, _password);
					client.Connect();
					client.DeleteFile(filePath);
					return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during file delete: {ex.Message}");

				return false;
			}
			finally
			{
				_semaphore.Release();
			}
		}
		//public byte[] DownloadFile(string filePath)
		//{
		//	filePath = $"/MedicloudV2/{filePath}";

		//	_semaphore.Wait();
		//	try
		//	{

		//		using (var client = new FtpClient(_ftpPath))
		//		{
		//			client.Credentials = new System.Net.NetworkCredential(_username, _password);
		//			client.Connect();
		//			if (client.FileExists(filePath))
		//			{
		//				client.DownloadBytes(out byte[] fileBytes, filePath);
		//				return fileBytes;
		//			}
		//			else
		//			{
		//				return Array.Empty<byte>();
		//			}
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine($"Error during file download: {ex.Message}");

		//		return Array.Empty<byte>();
		//	}
		//	finally
		//	{
		//		_semaphore.Release();
		//	}
		//}

		public async Task<byte[]> DownloadFileAsync(string filePath)
		{
			filePath = $"/MedicloudV2/{filePath}";

			await _semaphore.WaitAsync(); 
			try
			{
	
				return await Task.Run(() =>
				{
					using (var client = new FtpClient(_ftpPath))
					{
						client.Credentials = new System.Net.NetworkCredential(_username, _password);
						client.Connect();

						if (client.FileExists(filePath))
						{
							client.DownloadBytes(out byte[] fileBytes, filePath);
							return fileBytes;
						}
						else
						{
							return Array.Empty<byte>();
						}
					}
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error during file download: {ex.Message}");
				return Array.Empty<byte>();
			}
			finally
			{
				_semaphore.Release();  
			}
		}
	}

}
