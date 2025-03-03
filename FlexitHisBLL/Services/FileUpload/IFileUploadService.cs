
namespace Medicloud.BLL.Services.FileUpload
{
	public interface IFileUploadService
	{
		bool UploadFile(byte[] fileBytes, string filePath);
		byte[] DownloadFile(string filePath);
		public bool DeleteFile(string filePath);
	}
}
