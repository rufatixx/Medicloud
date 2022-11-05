using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore.Models;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.Login;
using FlexitHisMVC.Models.DTO;

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

            if (HttpContext.Session.GetInt32("userid") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return View();
                
            }
           
        }

        [HttpPost]
        public IActionResult SignIn(string username, string pass)
        {
            UserService login = new UserService(ConnectionString);
            var obj = login.SignIn(username, pass);
            
            if (obj.personal.ID > 0)
            {
                ResponseDTO<LogInDTO> response = new ResponseDTO<LogInDTO>();
                response.data = new List<LogInDTO>();
                response.data.Add(obj);

                HttpContext.Session.SetInt32("userid", obj.personal.ID);
                return Ok(response);
            }
            else {
                return Unauthorized();
            }
            
        }
    }
}

