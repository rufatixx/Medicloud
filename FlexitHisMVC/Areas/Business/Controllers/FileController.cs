using Medicloud.BLL.Services.File;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	public class FileController : Controller
	{
		private readonly IFileService _fileService;
		public FileController(IFileService fileService)
		{
			_fileService = fileService;
		}

		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> DeleteFile(int fileId)
		{
			await _fileService.DeleteById(fileId);
			return Ok();
		}

	}
}
