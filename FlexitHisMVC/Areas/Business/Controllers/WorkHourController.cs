using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.WorkHours;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
	[Area("Business")]
	public class WorkHourController : Controller
	{
		private readonly IWorkHourService _workHourService;

		public WorkHourController(IWorkHourService workHourService)
		{
			_workHourService = workHourService;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateWorkHours([FromBody] UpdateWorkHourDTO dto)
		{
			//int newId = await _staffService.UpdateStaffWorkHours(dto);
			await _workHourService.UpdateWorkHours(dto);
			return Ok();
		}
	}
}
