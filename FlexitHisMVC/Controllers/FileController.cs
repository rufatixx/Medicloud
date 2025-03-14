using Medicloud.BLL.Services.File;
using Medicloud.BLL.Services.FileUpload;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Controllers
{
	public class FileController : Controller
	{
		private readonly IFileService _fileService;
		private readonly IFileUploadService _fileUploadService;
		public FileController(IFileService fileService, IFileUploadService fileUploadService)
		{
			_fileService = fileService;
			_fileUploadService = fileUploadService;
		}


		public IActionResult Index()
		{
			return View();
		}

		public async Task<IActionResult> GetFile(int id)
		{
			var file = await _fileService.GetById(id);
			string filePath = $"/{file.filePath}";
			var fileBytes = await _fileUploadService.DownloadFileAsync(filePath);

			if (fileBytes != null && fileBytes.Length > 0)
			{
				var fileExtension = Path.GetExtension(filePath).ToLower();
				var fileName = $"{file.fileName}";

				string contentType = "application/octet-stream";

				switch (fileExtension)
				{
					case ".pdf":
						contentType = "application/pdf";
						break;
					case ".jpg":
					case ".jpeg":
						contentType = "image/jpeg";
						break;
					case ".png":
						contentType = "image/png";
						break;
					case ".txt":
						contentType = "text/plain";
						break;
				}

				return File(fileBytes, contentType);
			}
			return NotFound("Fayl tapılmadı");

		}

		public async Task<IActionResult> GetImage(int id)
		{
			var file = await _fileService.GetById(id);
			if (file != null && file.id>0)
			{
				string filePath = $"/{file.filePath}";
				var fileBytes = await _fileUploadService.DownloadFileAsync(filePath);

				if (fileBytes != null && fileBytes.Length > 0)
				{
					var fileExtension = Path.GetExtension(filePath).ToLower();
					var fileName = $"{file.fileName}";

					string contentType = "application/octet-stream";

					switch (fileExtension)
					{
						case ".jpg":
						case ".jpeg":
							contentType = "image/jpeg";
							break;
						case ".png":
							contentType = "image/png";
							break;
						case ".gif":
							contentType = "image/gif";
							break;
					}

					return File(fileBytes, contentType);
				}
			}

			return NotFound("Şəkil tapılmadı");

		}


		[HttpPost]
		public async Task<IActionResult> DeleteFile(int fileId)
		{
			await _fileService.DeleteById(fileId);
			return Ok();
		}

	}
}
