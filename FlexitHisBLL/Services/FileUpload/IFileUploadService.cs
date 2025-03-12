
namespace Medicloud.BLL.Services.FileUpload
{
	public interface IFileUploadService
	{
		Task<bool> UploadFileAsync(byte[] fileBytes, string filePath,bool isImage=false);
		//byte[] DownloadFile(string filePath);
		Task<byte[]> DownloadFileAsync(string filePath);
		public bool DeleteFile(string filePath);
	}
}
