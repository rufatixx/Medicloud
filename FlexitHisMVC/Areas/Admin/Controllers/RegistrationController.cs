using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.Organization;
using Medicloud.BLL.Services.Services;
using Medicloud.BLL.Services.Staff;
using Medicloud.BLL.Services.User;
using Medicloud.DAL.DAO;
using Medicloud.Models;
using Medicloud.ViewModels;
using Medicloud.WebUI.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
		private readonly IServicesService _servicesService;
		public RegistrationController(INUserService userService, ICategoryService categoryService, IOrganizationService organizationService, IStaffService staffService, IServicesService servicesService)
		{
			_userService = userService;
			_categoryService = categoryService;
			_organizationService = organizationService;
			_staffService = staffService;
			_servicesService = servicesService;
		}

		public async Task<IActionResult> Index(int organizationId = 0)
		{
			var categories = await _categoryService.GetAll();
			var vm = new CreateOrganizationVM
			{
				Categories = categories,
				SelectedCategories = new List<int>(),
				id = organizationId
			};

			if (organizationId > 0)
			{
				var orgCategories = await _categoryService.GetByOrganizationId(organizationId);
				vm.SelectedCategories = orgCategories.Select(c => c.id).ToList();
			}
			else
			{

				if (TempData["OrganizationVM"] != null)
				{

					var tempVm = JsonConvert.DeserializeObject<CreateOrganizationVM>(TempData["OrganizationVM"].ToString());
					if (tempVm?.SelectedCategories != null)
					{
						vm.SelectedCategories = tempVm.SelectedCategories;
					}
					TempData.Keep("OrganizationVM");

				}
			}
			Console.WriteLine(vm.SelectedCategories.Count);
			return View(vm);
		}

		[HttpGet]
		public async Task<IActionResult> Step2(int organizationId)
		{
			var vm = new CreateOrganizationVM
			{
				id = organizationId
			};
			if (organizationId == 0)
			{
				int userId = 187;
				var user = await _userService.GetUserById(userId);

				vm.UserId = userId;
				vm.StaffName = $"{user.name}";
				vm.StaffEmail = user.email ?? null;
				vm.StaffPhoneNumber = user.mobile ?? null;

				if (TempData["OrganizationVM"] != null)
				{
					var tempVm = JsonConvert.DeserializeObject<CreateOrganizationVM>(TempData["OrganizationVM"].ToString());
					TempData.Keep("OrganizationVM");
					vm.SelectedCategories = tempVm.SelectedCategories ?? new();

				}
				return View(vm);
			}



			var organization = await _organizationService.GetByIdAsync(organizationId);
			if (organization == null)
			{
				return RedirectToAction("Index", new { organizationId = vm.id });
			}

			vm.OrgName = organization.name;
			var staff = await _staffService.GetOwnerStaffByOrganizationId(organization.id);
			if (staff != null)
			{
				vm.UserId = staff.userId;
				vm.StaffPhoneNumber = staff.phoneNumber;
				vm.StaffEmail = staff.email;
				vm.StaffName = staff.name;
			}

			vm.OrgName = organization.name;
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Step2(CreateOrganizationVM vm)
		{
			if (vm == null)
			{
				return RedirectToAction("Index");
			}

			if (vm.SelectedCategories != null && vm.SelectedCategories.Any())
			{
				int userId = 187;
				if (userId == 0)
				{
					return RedirectToAction("Index", "Login");
				}

				if (vm.id > 0)
				{
					int updatedCategories = await _organizationService.UpdateOrganizationCategories(vm.id, vm.SelectedCategories);
					var organization = await _organizationService.GetByIdAsync(vm.id);

					if (organization == null)
					{
						return RedirectToAction("Index", new { organizationId = vm.id });
					}
				}
				else
				{
					TempData["OrganizationVM"] = JsonConvert.SerializeObject(vm);
				}

				return RedirectToAction("Step2", new { organizationId = vm.id });
			}
			else
			{
				return RedirectToAction("Index", new { organizationId = vm.id });
			}
		}

		[HttpGet]
		public async Task<IActionResult> Step3(int organizationId)
		{

			if (organizationId == 0)
			{
				return RedirectToAction("Index");
			}
			var vm = new CreateOrganizationVM
			{
				id = organizationId
			};
			var organization = await _organizationService.GetByIdAsync(organizationId);
			if (organization == null)
			{
				return RedirectToAction("Step2", new { organizationId });
			}

			vm.WorkPlaceType = (WorkPlaceType)organization.workPlaceType;

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Step3(CreateOrganizationVM vm)
		{
			if (vm == null)
			{
				return RedirectToAction("Index");
			}
			Console.WriteLine(vm.id);
			TempData.Clear();
			if (vm.id == 0)
			{
				var addDTO = new AddOrganizationDTO
				{
					SelectedCategories = vm.SelectedCategories,
					StaffEmail = vm.StaffEmail,
					StaffName = vm.StaffName,
					StaffPhoneNumber = vm.StaffPhoneNumber,
					Name = vm.OrgName,
					UserId = 187
				};
				int newOrgId = await _organizationService.AddAsync(addDTO);
				if (newOrgId == 0)
				{
					RedirectToAction("Step2", new { selectedCategories = vm.SelectedCategories });
				}
				vm.id = newOrgId;
			}
			else
			{


				Console.WriteLine(vm.id);
				var organization = await _organizationService.GetByIdAsync(vm.id);
				if (organization == null)
				{
					return RedirectToAction("Step2", new { organizationId = vm.id });
				}
				var staff = await _staffService.GetOwnerStaffByOrganizationId(organization.id);


				var dao = new StaffDAO
				{
					id = staff.id,
					email = vm.StaffEmail,
					name = vm.StaffName,
					phoneNumber = vm.StaffPhoneNumber,
					permissionLevelId = 1,
				};

				bool isUpdated = await _staffService.UpdateStaffAsync(dao);
				await _organizationService.UpdateAsync(new()
				{
					id = vm.id,
					name = vm.OrgName
				});

				vm.WorkPlaceType = (WorkPlaceType)organization.workPlaceType;

			}
			return RedirectToAction("Step3", new { organizationId = vm.id });

		}

		[HttpGet]
		public async Task<IActionResult> Step4(int organizationId)
		{
			if (organizationId == 0)
			{
				return RedirectToAction("Index");
			}
			var vm = new CreateOrganizationVM
			{
				id = organizationId
			};

			var organization = await _organizationService.GetByIdAsync(organizationId);
			if (organization == null)
			{
				RedirectToAction("Step2", new { organizationId });
			}
			if (organization != null)
			{
				vm.OrgAddress = organization.address;
				vm.latitude = organization.latitude;
				vm.longitude = organization.longitude;
			}

			return View(vm);

		}

		[HttpPost]
		public async Task<IActionResult> Step4(CreateOrganizationVM vm)
		{
			if (vm.id == 0)
			{
				return RedirectToAction("Index");
			}

			var updateDAO = new OrganizationDAO
			{
				id = vm.id,
				workPlaceType = (int)vm.WorkPlaceType,
			};
			bool isUpdated = await _organizationService.UpdateAsync(updateDAO);

			return RedirectToAction("Step4", new { organizationId = vm.id });

		}

		[HttpGet]
		public async Task<IActionResult> WorkPlaceTypeSwitch(int organizationId)
		{
			Console.WriteLine($"WorkPlaceTypeSwitch get {organizationId.ToString()}");
			if (organizationId == 0)
			{
				return RedirectToAction("Index");
			}

			var organization = await _organizationService.GetByIdAsync(organizationId);
			if (organization == null)
			{
				return RedirectToAction("Step4", new { organizationId });
			}
			if (organization?.workPlaceType > 0 && (WorkPlaceType)organization.workPlaceType != WorkPlaceType.MyLocation)
			{
				return RedirectToAction("Step5", new { organizationId });
			}

			return RedirectToAction("Step7", new { organizationId });
		}

		[HttpGet]
		public async Task<IActionResult> Step5(int organizationId)
		{
			Console.WriteLine($"step5 get {organizationId.ToString()}");
			if (organizationId == 0)
			{
				return RedirectToAction("Index");
			}

			var organization = await _organizationService.GetByIdAsync(organizationId);
			var organizationTravel = await _organizationService.GetOrganizationTravel(organizationId);
			if (organization == null)
			{
				return RedirectToAction("Step4", new { organizationId });
			}

			var vm = new CreateOrganizationVM
			{
				id = organization.id,
				TravelDistance=(int)organizationTravel.distance,
				TravelPrice=organizationTravel.fee,
				TravelPriceType=organizationTravel.feeType,
				TravelId=organizationTravel.id,
				OrgAddress = organization.address,
				latitude = organization.latitude,
				longitude = organization.longitude,

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
				latitude = vm.latitude,
				longitude = vm.longitude,
				address = vm.OrgAddress,
			};

			var updated = await _organizationService.UpdateAsync(updateDAO);
			return RedirectToAction("WorkPlaceTypeSwitch", new { organizationId = vm.id });

		}
		[HttpPost]
		public async Task<IActionResult> Step6(CreateOrganizationVM vm)
		{
			if (vm == null)
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
			if (vm.TravelId == 0)
			{
				var newOrgTravelId = await _organizationService.AddOrganizationTravel(organizationTravelDAO);
			}
			else
			{
				organizationTravelDAO.id = vm.TravelId;
				bool updated=await _organizationService.UpdateOrganizationTravel(organizationTravelDAO);
			}


			return RedirectToAction("Step7", new { organizationId = vm.id });

		}

		[HttpGet]
		public async Task<IActionResult> Step7(int organizationId)
		{
			Console.WriteLine($"step5 get {organizationId.ToString()}");


			var organization = await _organizationService.GetByIdAsync(organizationId);
			if (organization == null)
			{
				return RedirectToAction("Index");
			}

			var model = new CreateOrganizationVM
			{
				id = organizationId,
				TeamSizeId = organization.teamSizeId,
				WorkPlaceType = (WorkPlaceType)organization.workPlaceType,
			};
			return View(model);

		}

		[HttpGet]
		public async Task<IActionResult> Step8(int organizationId = 29)
		{
			var staff = await _staffService.GetOwnerStaffByOrganizationId(organizationId);
			var staffWorkHours = await _staffService.GetWorkHours(staff.id);
			var vm = new CreateOrganizationVM
			{
				id = organizationId,
				WorkHours = staffWorkHours,
			};
			return View(vm);

		}
		[HttpPost]
		public async Task<IActionResult> Step8(CreateOrganizationVM vm)
		{


			if (vm.id == 0 || vm.TeamSizeId == 0)
			{
				return RedirectToAction("Index");
			}
			var updateDAO = new OrganizationDAO
			{
				teamSizeId = vm.TeamSizeId,
				id = vm.id,
			};
			bool isUpdated = await _organizationService.UpdateAsync(updateDAO);
			return RedirectToAction("Step8", new { organizationId = vm.id });

		}

		[HttpGet]
		public async Task<IActionResult> Step9(int organizationId = 29)
		{
			Console.WriteLine($"step5 get {organizationId.ToString()}");

			var data = await _servicesService.GetServiceTypes();
			var services = await _servicesService.GetServicesByOrganizationAsync(organizationId);
			var vm = new CreateOrganizationVM
			{
				id = organizationId,
				ServiceTypes = data,
				Services = services,
				hasTravel = false
			};
			return View(vm);

		}
		[HttpGet]
		public async Task<IActionResult> Success(int organizationId = 29)
		{

			return View();

		}
	}
}
