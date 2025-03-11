
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.FileUpload;
using Medicloud.BLL.Services.Portfolio;
using Medicloud.WebUI.Areas.Business.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	public class PortfolioController : Controller
	{
		private readonly IPortfolioService _portfolioService;
		private readonly ICategoryService _categoryService;
		private IFileUploadService _fileUploadService;
		public PortfolioController(IPortfolioService portfolioService, ICategoryService categoryService, IFileUploadService fileUploadService)
		{
			_portfolioService = portfolioService;
			_categoryService = categoryService;
			_fileUploadService = fileUploadService;
		}

		public async Task<IActionResult> Index()
		{
			var portfolios = await _portfolioService.GetPortfolioByOrganizationIdAsync(41);
			var vm = new PortfolioViewModel()
			{
				organizationId = 41,
				portfolios = portfolios
			};
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> AddPortfolioPhoto([FromForm] IFormFile photo, int organizationId)
		{
			Console.WriteLine(organizationId);
			var vm = new AddPortfolioViewModel();

			string filePath = null;
			byte[] file;
			string fileExtension;
			if (photo != null && photo.Length != 0)
			{
				using (var memoryStream = new MemoryStream())
				{
					await photo.CopyToAsync(memoryStream);

					file = memoryStream.ToArray();
				}
				fileExtension = Path.GetExtension(photo.FileName)?.ToLower();
				var base64String = Convert.ToBase64String(file);
				vm.photo = base64String;
				vm.extension = fileExtension;
				vm.organizationId = organizationId;
				vm.photoSrc = $"data:image/{fileExtension};base64,{base64String}";

				var categories = await _categoryService.GetByOrganizationId(organizationId);
				vm.Categories = categories;
				return View("AddPortfolio", vm);

				//if (string.IsNullOrEmpty(fileExtension))
				//{
				//	return BadRequest("Invalid file extension.");
				//}

				//string fileName = Guid.NewGuid().ToString();
				//filePath = $"PortfolioImages/organization{organizationId}_{fileName}{fileExtension}";
				//bool uploaded = _fileUploadService.UploadFile(file, filePath);
				//if (uploaded)
				//{
				//	//int newFileId = await _fileService.AddAsync(new()
				//	//{
				//	//	fileName = fileName,
				//	//	filePath = filePath,
				//	//});
				//	//TempData["FileId"] = newFileId;
				//	//TempData["OrganizationId"] = organizationId;
				//	//return RedirectToAction("AddPortfolio");

				//}

			}

			var controllerName = (string)HttpContext.GetRouteData().Values["controller"];

			return Redirect($"{controllerName}/Index");

		}

		//[HttpGet]
		//public async Task<IActionResult> AddPortfolio()
		//{
		//	var vm = new AddPortfolioViewModel();
		//	//int fileId = (int)TempData["FileId"];
		//	int fileId = 4;
		//	var file = await _fileService.GetById(fileId);
		//	if (file != null)
		//	{
		//		var fileData = _fileUploadService.DownloadFile(file.filePath);
		//		if (fileData != null)
		//		{
		//			Console.WriteLine(fileData.Length);
		//			var extension = Path.GetExtension(file.fileName)?.ToLower();
		//			var base64String = Convert.ToBase64String(fileData);

		//			//organizationId = (int)TempData["OrganizationId"],
		//			vm.organizationId = 41;
		//			vm.photo = base64String;
		//			vm.photoSrc = $"data:image/{extension};base64,{base64String}";
		//			var categories = await _categoryService.GetByOrganizationId(41);
		//			vm.Categories = categories;
		//		}
		//	}



		//	return View(vm);

		//}

		[HttpPost]
		public async Task<IActionResult> AddPortfolio(AddPortfolioViewModel vm)
		{


			string filePath = null;
			byte[] file;
			string fileExtension;
			if (vm.photo != null && vm.photo.Length != 0)
			{
				file = Convert.FromBase64String(vm.photo);
				var fileName = Guid.NewGuid().ToString();
				filePath = $"PortfolioImages/organization_{vm.organizationId}_{fileName}{vm.extension}";
				bool uploaded = _fileUploadService.UploadFile(file, filePath);
				if (uploaded)
				{
					int newId = await _portfolioService.AddPortfolioAsync(new()
					{
						file = new()
						{
							filePath = filePath,
							fileName = fileName,
						},
						description = vm.description,
						organizationId = vm.organizationId,
						categoryIds = string.Join(",", vm.selectedCategoryIds)

					});
				}
			}

			return RedirectToAction("Index");

		}
	}
}
