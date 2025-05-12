using Medicloud.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
	public class AnamnesisDetailModalViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
