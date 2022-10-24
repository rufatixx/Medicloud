using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHis_API.Models.Db;
using FlexitHis_API.Models.Structs;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Controllers
{
 
    public class LoginController : Controller
    {
      
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public LoginController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("login/signIn")]
        public ActionResult<LogInStruct> SignIn(string username, string pass)
        {
            Select select = new Select(Configuration, _hostingEnvironment);
            var result = select.LogIn(username, pass);
            if (result.status == 1)
            {
                HttpContext.Session.SetInt32("userid", result.id);
            }
            return select.LogIn(username, pass);

        }
    }
}

