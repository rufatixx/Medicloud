using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Controllers
{
    [Authorize]
    public class CalendarController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
