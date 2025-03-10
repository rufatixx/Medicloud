using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.ViewComponents
{
	public class PhotoViewViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
