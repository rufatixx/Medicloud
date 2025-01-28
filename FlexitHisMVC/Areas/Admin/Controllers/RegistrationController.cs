using Medicloud.BLL.Services.Category;
using Medicloud.BLL.Services.User;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class RegistrationController : Controller
	{
		private readonly INUserService _userService;
		private readonly ICategoryService _categoryService;
		public RegistrationController(INUserService userService, ICategoryService categoryService)
		{
			_userService = userService;
			_categoryService = categoryService;
		}

		public async Task<IActionResult> Index(int userId)
		{
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

			};

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Step2(CreateOrganizationVM vm)
		{
			if(vm == null)
			{
				return RedirectToAction("Index","Login");
			}

			vm.categories = await _categoryService.GetAll();
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Step3(CreateOrganizationVM vm)
		{
			if (vm == null)
			{
				return RedirectToAction("Index", "Login");
			}

			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> Step4(CreateOrganizationVM vm)
		{
			if (vm == null)
			{
				return RedirectToAction("Index", "Login");
			}

			return View(vm);
		}
		[HttpPost]
		public async Task<IActionResult> Step5(CreateOrganizationVM vm)
		{
			if (vm == null)
			{
				return RedirectToAction("Index", "Login");
			}

			return View(vm);
		}
	}
}
