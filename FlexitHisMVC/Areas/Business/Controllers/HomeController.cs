using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Business.Controllers
{
    //[Authorize]
    [Area("Business")]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        public HomeController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
       
        // GET: /<controller>/
        public IActionResult Index()
        {
			var orgIdString = HttpContext.Session.GetString("CurrentOrganization");
			int currentOrganizationId = int.Parse(orgIdString??"1");
			if(currentOrganizationId > 0)
			{
				return View();

			}
			return RedirectToAction("Index","Home");

		}

    }
}

