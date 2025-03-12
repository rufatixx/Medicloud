using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	public class SettingsController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
		public async Task<IActionResult> Details()
		{
			return View();
		}
	}
}
