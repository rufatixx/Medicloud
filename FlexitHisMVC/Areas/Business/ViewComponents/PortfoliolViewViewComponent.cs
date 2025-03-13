using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.ViewComponents
{
	
	public class PortfolioViewViewComponent:ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{
			return View();
		}
	}
}
