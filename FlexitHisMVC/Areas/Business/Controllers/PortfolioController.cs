
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.FileUpload;
using Medicloud.BLL.Services.Portfolio;
using Medicloud.WebUI.Areas.Business.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	[Authorize]
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
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;
			var portfolios = await _portfolioService.GetPortfolioByOrganizationIdAsync(activeOrganizationId);
			var vm = new PortfolioViewModel()
			{
				organizationId = activeOrganizationId,
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
				vm.selectedCategoryIds = new();
				var categories = await _categoryService.GetByOrganizationId(organizationId);
				vm.Categories = categories;
				return View("AddPortfolio", vm);

			}

			var controllerName = (string)HttpContext.GetRouteData().Values["controller"];

			return Redirect($"{controllerName}/Index");

		}

		[HttpGet]
		public async Task<IActionResult> AddPortfolio(int id)
		{
			var portfolio = await _portfolioService.GetPortfolioByIdAsync(id);
			List<int> CategoryIds = portfolio.categoryIds.Split(',').Select(c => int.Parse(c)).ToList();

			var vm = new AddPortfolioViewModel()
			{
				id = portfolio.id,
				description = portfolio.description,
				photoSrc = Url.Action("GetImage", "File", new { area = "", id = portfolio.fileId }),
				Categories = await _categoryService.GetByOrganizationId(41),
				selectedCategoryIds = portfolio.categoryIds.Split(',').Select(c => int.Parse(c)).ToList() ?? new(),
				isEdit = true,
				organizationId = portfolio.organizationId,

			};

			return View(vm);

		}

		[HttpPost]
		public async Task<IActionResult> AddPortfolio(AddPortfolioViewModel vm)
		{


			string filePath = null;
			byte[] file;
			string fileExtension;
			if (!vm.isEdit)
			{
				if (vm.photo != null && vm.photo.Length != 0)
				{
					file = Convert.FromBase64String(vm.photo);
					var fileName = Guid.NewGuid().ToString();
					filePath = $"PortfolioImages/organization_{vm.organizationId}_{fileName}{vm.extension}";
					bool uploaded = await _fileUploadService.UploadFileAsync(file, filePath, true);
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
							categoryIds = string.Join(",", vm.selectedCategoryIds ?? new())

						});
					}
				}
			}
			else
			{
				await _portfolioService.UpdateAsync(new()
				{
					categoryIds = string.Join(',', vm.selectedCategoryIds??new()),
					id = vm.id,
					description = vm.description,
				});
			}


			return RedirectToAction("Index");

		}

		[HttpPost]
		public async Task<IActionResult> DeletePortfolio(int id)
		{
			await _portfolioService.DeleteAsync(id);

			return Ok();

		}
	}
}
