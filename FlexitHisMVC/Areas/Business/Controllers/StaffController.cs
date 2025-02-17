using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.Staff;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.WebUI.Areas.Business.Controllers
{
    [Area("Business")]
    public class StaffController : Controller
    {
        private readonly IStaffService _staffService;
        public StaffController(IStaffService staffService)
        {
            _staffService=staffService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStaffWorkHours([FromBody] UpdateStaffWorkHourDTO dto)
        {
            //int newId = await _staffService.UpdateStaffWorkHours(dto);
            await _staffService.UpdateStaffWorkHours(dto);
            return Ok();
        }
    }
}
