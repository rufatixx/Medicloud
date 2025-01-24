using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class WelcomeController : Controller
    {
 
        public WelcomeController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");

			}
			return View();
          
        }

    

      

      


    }
}

