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
		
        public async Task<IActionResult> Index()
        {

            return View();
        }
    }
}
