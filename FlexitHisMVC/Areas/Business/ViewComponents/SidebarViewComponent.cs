using Medicloud.BLL.Services.Organization;
using Medicloud.WebUI.Areas.Business.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;

namespace Medicloud.WebUI.Areas.Business.ViewComponents
{
	public class SidebarViewComponent:ViewComponent
	{
		private readonly IOrganizationService _organizationService;

		public SidebarViewComponent(IOrganizationService organizationService)
		{
			_organizationService = organizationService;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{



			int activeOrganizationId = HttpContext.Session.GetInt32("activeOrgId") ?? 0;

			int userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
			var organizations = await _organizationService.GetUserOrganizations(userId);

			//Console.WriteLine(activeOrganizationId);
			var activeOrg=organizations.FirstOrDefault(o=>o.Id == activeOrganizationId);

			var vm = new SidebarViewModel
			{
				Organizations=organizations.Where(o=>o.Id!=activeOrganizationId).ToList(),
				ActiveId=activeOrganizationId,
				OrganizationName=activeOrg?.Name,
				LogoId = activeOrg?.LogoId??0,
				StaffName = activeOrg?.StaffName,
			};
			return View(vm);

			//if (organizations != null && organizations.Count > 0)
			//{
			//	var active =activeOrganizationId>0?organizations.First(o=>o.Id==activeOrganizationId): organizations.Last();
			//	//HttpContext.Session.SetInt32("activeOrgId", active.Id);
			//}
			//var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			//int userId = 0;
			//if (userIdClaim != null)
			//	userId = int.Parse(userIdClaim);

			//var user = await _userService.GetUserAsync(userId);

			//var userModuls = await _accessService.GetUserModulesAsync(userId);
			//var userModuleEnums = userModuls
			//.Select(m => Enum.IsDefined(typeof(UserModule), m.id) ? (UserModule?)m.id : null)
			//.Where(module => module.HasValue)
			//.ToList();

			//byte[] file = null;
			//if (!string.IsNullOrEmpty(user.photo_path))
			//{
			//	file = _fileUploadService.DownloadFile(user.photo_path);
			//}

			//string imageSrc = "/user_images/default-user.svg";
			//if (file != null && file.Length > 0)
			//{
			//	var base64String = Convert.ToBase64String(file);
			//	string fileExtension = Path.GetExtension(user.photo_path)?.ToLower();
			//	imageSrc = $"data:image/{fileExtension};base64,{base64String}";
			//}
		}


	}
}
