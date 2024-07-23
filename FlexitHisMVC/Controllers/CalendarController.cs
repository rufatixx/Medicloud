using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Controllers
{
	public class CalendarController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
