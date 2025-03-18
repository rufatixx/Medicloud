using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.Organization;
using Medicloud.Models;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	[Authorize]
	public class SettingsController : Controller
	{
		private readonly ICategoryService _categoryService;
		private readonly IOrganizationService _organizationService;

		public SettingsController(ICategoryService categoryService, IOrganizationService organizationService)
		{
			_categoryService = categoryService;
			_organizationService = organizationService;
		}

		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Details()
		{
			return View();
		}
		public async Task<IActionResult> Category()
		{
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;
			var categories = await _categoryService.GetAll();
			var vm = new CreateOrganizationVM
			{
				Categories = categories,
				SelectedCategories = new List<int>(),
				id = activeOrganizationId
			};

			if (activeOrganizationId > 0)
			{
				var orgCategories = await _categoryService.GetByOrganizationId(activeOrganizationId);
				vm.SelectedCategories = orgCategories.Select(c => c.id).ToList();
			}
			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Category(CreateOrganizationVM vm)
		{
			if (vm!=null && vm.SelectedCategories != null && vm.SelectedCategories.Any())
			{

				int updatedCategories = await _organizationService.UpdateOrganizationCategories(vm.id, vm.SelectedCategories);
			}
			return RedirectToAction("Category");
		}


		public async Task<IActionResult> WorkHours()
		{
			//int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;

			//var organization = await _organizationService.GetByIdAsync(activeOrganizationId);
			//if (organization == null || organization.isRegistered)
			//{
			//	return RedirectToAction("Index");
			//}
			//var staff = await _staffService.GetOwnerStaffByOrganizationId(activeOrganizationId);
			//var staffWorkHours = await _staffService.GetWorkHours(staff.id);
			//var vm = new CreateOrganizationVM
			//{
			//	id = organizationId,
			//	WorkHours = staffWorkHours,
			//	StaffId = staff.id,
			//};
			return View();
		}


		public async Task<IActionResult> VenueAndLocation()
		{
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;
			var vm = new CreateOrganizationVM
			{
				id = activeOrganizationId
			};

			var organization = await _organizationService.GetByIdAsync(activeOrganizationId);
			if (organization != null)
			{
				vm.OrgAddress = organization.address;
				vm.latitude = organization.latitude;
				vm.longitude = organization.longitude;
			}
			return View(vm);
		}
	}
}
