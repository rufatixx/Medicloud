using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.Organization;
using Medicloud.BLL.Services.Staff;
using Medicloud.BLL.Services.User;
using Medicloud.DAL.DAO;
using Medicloud.ViewModels;
using Medicloud.WebUI.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Areas.Admin.Controllers
{
	[Area("Admin")]
	//[Authorize]
	public class RegistrationController : Controller
	{
		private readonly INUserService _userService;
		private readonly ICategoryService _categoryService;
		private readonly IOrganizationService _organizationService;
		private readonly IStaffService _staffService;
		public RegistrationController(INUserService userService, ICategoryService categoryService, IOrganizationService organizationService, IStaffService staffService)
		{
			_userService = userService;
			_categoryService = categoryService;
			_organizationService = organizationService;
			_staffService = staffService;
		}

		public async Task<IActionResult> Index()
		{

			var categories = await _categoryService.GetAll();
			return View(categories);
		}
		[HttpPost]
		public async Task<IActionResult> Step2(List<int>selectedCategories)
		{
			if (selectedCategories.Any())
			{
				//foreach (var category in selectedCategories)
				//{
				//	Console.WriteLine($"ct {category.ToString()}");
				//}
				int userId = 187;
				if (userId == 0)
				{
					return RedirectToAction("Index", "Login");
				}
				var user = await _userService.GetUserById(userId);

				var vm = new CreateOrganizationVM
				{
					UserId = userId,
					StaffName = $"{user.name}",
					StaffEmail = user.email ?? null,
					StaffPhoneNumber = user.mobile ?? null,
					SelectedCategories = selectedCategories

				};
				return View(vm);
			}
			else
			{
				return RedirectToAction("Index");
			}



		}

		[HttpPost]
		public async Task<IActionResult> Step3(CreateOrganizationVM vm)
		{
			if (vm == null)
			{
				return RedirectToAction("Index");
			}

			var addDTO = new AddOrganizationDTO
			{
				SelectedCategories = vm.SelectedCategories,
				StaffEmail = vm.StaffEmail,
				StaffName = vm.StaffName,
				StaffPhoneNumber = vm.StaffPhoneNumber,
				Name = vm.OrgName,
				UserId = vm.UserId
			};

			int newOrgId=await _organizationService.AddAsync(addDTO);
			if(newOrgId == 0)
			{
				RedirectToAction("Step2",new {selectedCategories=vm.SelectedCategories});
			}
			vm.id=newOrgId;

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Step4(int orgId=0,int workPlaceType=1)
		{
			if (orgId == 0)
			{
				return RedirectToAction("Index");
			}

			var updateDAO = new OrganizationDAO
			{
				id = orgId,
				workPlaceType = workPlaceType
			};
			bool isUpdated = await _organizationService.UpdateAsync(updateDAO);
			var vm = new CreateOrganizationVM
			{
				id = orgId,
				WorkPlaceType=(WorkPlaceType)workPlaceType

			};
			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Step5(CreateOrganizationVM vm)
		{
			if (vm == null)
			{
				return RedirectToAction("Index");
			}
			Console.WriteLine($"long {vm.longitude.ToString()}");
			Console.WriteLine($"long {vm.latitude.ToString()}");
			Console.WriteLine($"add {vm.OrgAddress?.ToString()}");
			Console.WriteLine($"add {vm.WorkPlaceType.ToString()}");
			var updateDAO = new OrganizationDAO
			{
				id = vm.id,
				latitude=vm.latitude,
				longitude=vm.longitude,
				address=vm.OrgAddress,
			};

			var updated=await _organizationService.UpdateAsync(updateDAO);
			if(vm.WorkPlaceType==WorkPlaceType.ClientLocation || vm.WorkPlaceType == WorkPlaceType.Both)
			{

				return View(vm);
			}
			else
			{
				return RedirectToAction("Step7", new { orgId = vm.id });
			}
		}
		[HttpPost]
		public async Task<IActionResult> Step6(CreateOrganizationVM vm)
		{
			if(vm== null)
			{
				return RedirectToAction("Index");
			}
			var organizationTravelDAO = new OrganizationTravelDAO
			{
				organizationId = vm.id,
				distance = vm.TravelDistance,
				fee = vm.TravelPrice,
				feeType = vm.TravelPriceType
			};
			var newOrgTravelId = await _organizationService.AddOrganizationTravel(organizationTravelDAO);

			return RedirectToAction("Step7", new { orgId = vm.id });

		}

		[HttpGet]
		public async Task<IActionResult> Step7(int orgId)
		{
			var organization=await _organizationService.GetByIdAsync(orgId);
			if (organization == null)
			{
				return RedirectToAction("Index");
			}

			var model = new CreateOrganizationVM
			{

			};
			return View(organization.id);

		}

		[HttpPost]
		public async Task<IActionResult> Step8(int orgId=0,int teamSize=0)
		{
			if (orgId == 0 || teamSize == 0)
			{
				return RedirectToAction("Index");
			}
			var updateDAO = new OrganizationDAO
			{
				teamSizeId = teamSize,
				id = orgId,
			};
			bool isUpdated = await _organizationService.UpdateAsync(updateDAO);

			var staff=await _staffService.GetOwnerStaffByOrganizationId(orgId);

			return View(staff.id);

		}
	}
}
