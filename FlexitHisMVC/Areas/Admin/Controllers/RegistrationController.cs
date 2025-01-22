using Medicloud.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class RegistrationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
		[HttpGet]
		public async Task<IActionResult> Step1()
		{
			return View(new CreateOrganizationVM());
		}
	}
}
