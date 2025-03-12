using Medicloud.BLL.Services.Organization;
using Medicloud.WebUI.Areas.Business.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
			var organization = await _organizationService.GetByIdAsync(41);

			var vm = new SidebarViewModel
			{
				OrganizationName=organization.name,
				LogoId=organization.logoId,
			};
			return View(vm);
		}


	}
}
