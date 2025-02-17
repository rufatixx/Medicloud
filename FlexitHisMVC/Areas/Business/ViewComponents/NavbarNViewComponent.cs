using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.ViewComponents
{
	[ViewComponent]
	public class NavbarNViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{

			return View();
		}
	}
}
