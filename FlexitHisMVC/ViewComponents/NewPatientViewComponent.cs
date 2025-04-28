using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
	public class NewPatientViewComponent : ViewComponent
	{
		public async Task<IViewComponentResult> InvokeAsync()
		{

			return View();
		}
	}
}
