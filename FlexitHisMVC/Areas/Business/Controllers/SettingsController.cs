using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.AmenityService;
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.Organization;
using Medicloud.BLL.Services.Staff;
using Medicloud.BLL.Services.WorkHours;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	[Authorize]
	public class SettingsController : Controller
	{
		private readonly ICategoryService _categoryService;
		private readonly IOrganizationService _organizationService;
		private readonly IWorkHourService _workHourService;
		private readonly IStaffService _staffService;
		private readonly IAmenityService _amenityService;
		public SettingsController(ICategoryService categoryService, IOrganizationService organizationService, IWorkHourService workHourService, IStaffService staffService, IAmenityService amenityService)
		{
			_categoryService = categoryService;
			_organizationService = organizationService;
			_workHourService = workHourService;
			_staffService = staffService;
			_amenityService = amenityService;
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
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;

			var organization = await _organizationService.GetByIdAsync(activeOrganizationId);
			var workHours = await _workHourService.GetOrganizationWorkHours(organization.id);
			var vm = new CreateOrganizationVM
			{
				id = activeOrganizationId,
				WorkHours = workHours,
			};
			return View(vm);
		}


		[HttpPost]
		public async Task<IActionResult> UpdateWorkHours(CreateOrganizationVM vm)
		{
			var openedDaysList = JsonConvert.DeserializeObject<List<int>>(vm.OpenedDays);
			var closedDaysList = JsonConvert.DeserializeObject<List<int>>(vm.ClosedDays);
			var dto = new UpdateWorkHourDTO
			{
				ClosedDays = closedDaysList,
				OpenedDays = openedDaysList
			};
			var updated = await _workHourService.UpdateWorkHours(dto);
			return RedirectToAction("WorkHours");

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


		public async Task<IActionResult> Amenities()
		{
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;
			var result=await _amenityService.GetOrganizationAmenitiesAsync(activeOrganizationId);
			return View(result);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAmenities(OrganizationAmenityDTO dto)
		{
			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;
			await _amenityService.UpdateOrganizationAmenitiesAsync(dto);
			return RedirectToAction("Amenities");
		}
	}
}
