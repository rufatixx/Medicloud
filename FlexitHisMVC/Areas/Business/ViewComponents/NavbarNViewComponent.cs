using Medicloud.BLL.Services.Organization;
using Medicloud.BLL.Services.User;
using Medicloud.WebUI.Areas.Business.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Medicloud.WebUI.Areas.Business.ViewComponents
{
	[ViewComponent]
	public class NavbarNViewComponent : ViewComponent
	{
		private readonly IOrganizationService _organizationService;
		private readonly INUserService _userService;

		public NavbarNViewComponent(IOrganizationService organizationService, INUserService userService)
		{
			_organizationService = organizationService;
			_userService = userService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			int userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

			var user = await _userService.GetUserById(userId);

			//var organizations = await _organizationService.GetUserOrganizations(userId);
			//if (organizations != null && organizations.Count > 0)
			//{
			//	var active = organizations.Last();
			//	HttpContext.Session.SetInt32("activeOrgId", active.Id);

			//}
			var vm = new NavbarViewModel
			{
				UserEmail = string.IsNullOrEmpty(user?.email)?user?.mobile:user?.email,
				UserId = userId,
			};
			return View(vm);
		}

	}


}
