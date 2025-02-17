using Medicloud.BLL.Services.Category;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Areas.Business.Controllers
{
	[Area("Business")]
	public class OrganizationController : Controller
    {
		private readonly ICategoryService _categoryService;

		public OrganizationController(ICategoryService categoryService)
		{
			_categoryService = categoryService;
		}

		public IActionResult Index()
        {
            return View();
        }
		[HttpGet]
		public async Task<IActionResult> Step1()
		{
			return View(new CreateOrganizationVM());
		}
		public async Task<IActionResult> Create()
		{
			var categories = await _categoryService.GetAll();
			return View(categories);
		}

	}
}
