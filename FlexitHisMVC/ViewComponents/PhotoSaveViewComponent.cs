using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Medicloud.WebUI.ViewComponents
{
	public class PhotoSaveViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
