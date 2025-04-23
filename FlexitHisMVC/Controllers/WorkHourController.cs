using Medicloud.BLL.Services.WorkHour;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Controllers
{
	public class WorkHourController : Controller
	{
		private readonly IWorkHourService _workHourService;

		public WorkHourController(IWorkHourService workHourService)
		{
			_workHourService = workHourService;
		}

		//public IActionResult Index()
		//{
		//	return View();
		//}
		[HttpGet]
		public async Task<IActionResult> GetUserWorkHours(int userId,DateTime selectedDay)
		{
			int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			var userWorkHours = await _workHourService.GetOrganizationUserWorkHours(userId, organizationId);
			int dayWeek = (int)selectedDay.DayOfWeek;
			var dayWorkHours = userWorkHours.FirstOrDefault(w => w.dayOfWeek == dayWeek);
			return Ok(dayWorkHours);
		}
	}
}
