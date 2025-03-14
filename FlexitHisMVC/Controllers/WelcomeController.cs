using Medicloud.BLL.Services.Organization;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class WelcomeController : Controller
    {

		private readonly IOrganizationService _organizationService;

		public WelcomeController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IOrganizationService organizationService)
		{
			_organizationService = organizationService;
		}
		// GET: /<controller>/
		public  async Task<IActionResult> Index()
        {

			if (User.Identity.IsAuthenticated)
			{
				int userId = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

				var organizations = await _organizationService.GetUserOrganizations(userId);
				if (organizations != null && organizations.Count > 0)
				{

					var active = organizations.Last();
					HttpContext.Session.SetInt32("activeOrgId", active.Id);
				}
				else
				{
					return RedirectToAction("Index", "Registration", new { area = "business" });
				}
				return RedirectToAction("Index", "Home",new { area="business" });

			}
			return View();
          
        }

    

      

      


    }
}

