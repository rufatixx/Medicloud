using System.Security.Claims;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.User;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class LoginController : Controller
    {

        private readonly string _connectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        UserService userService;
        private readonly INUserService _nUserService;
        public LoginController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, INUserService userService)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            //userService = new UserService(_connectionString);
            _nUserService=userService;
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

        //    [HttpPost("[controller]/[action]")]
        //    public async Task<IActionResult> SignInAsync(string mobileNumber, string pass)
        //    {
        //        UserService login = new UserService(_connectionString);
        //        var obj = login.SignIn(mobileNumber, pass);

        //        if (obj.personal.ID > 0)
        //        {
        //            var claims = new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Name, $"{obj.personal.name} {obj.personal.surname}"),
        //            new Claim("ID",obj.personal.ID.ToString())
        //            // Add additional claims as needed
        //        };

        //            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        //            var authProperties = new AuthenticationProperties
        //            {
        //                IsPersistent = true, // Remember the user across sessions
        //            };

        //            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


        //            ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>();
        //            response.data = new List<UserDTO>();
        //            response.data.Add(obj);


        //            //HttpContext.Session.SetInt32("userid", obj.personal.ID);




        //            userService.SaveSession(HttpContext, "Medicloud_userID", obj.personal.ID.ToString());
        //if (obj.organizations!=null && obj.organizations.Count>0)
        //{
        //	userService.SaveSession(HttpContext, "Medicloud_organizationID", obj.organizations[0].organizationID.ToString());
        //	userService.SaveSession(HttpContext, "Medicloud_organizationName", obj.organizations[0].organizationName.ToString());

        //}
        ////userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", obj.personal.subscription_expire_date.ToString());
        //userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", DateTime.Now.AddDays(10).ToString());
        //            return Ok(response);
        //        }
        //        else
        //        {
        //            return Unauthorized();
        //        }

        //    }


        [HttpPost("[controller]/[action]")]
        public async Task<IActionResult> SignInAsync(string contact, int contactType, string pass)
        {
            var user = await _nUserService.SignInAsync(contact, contactType, pass);
            if (user == null)
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, $"{user.name} {user.surname}"),
                    new Claim("ID",user.id.ToString())
                    // Add additional claims as needed
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // Remember the user across sessions
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


            ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>();
            response.data = new List<UserDTO>();
            //response.data.Add(obj);


            //HttpContext.Session.SetInt32("userid", obj.personal.ID);




            //userService.SaveSession(HttpContext, "Medicloud_userID", user.id.ToString());
            //if (obj.organizations!=null && obj.organizations.Count>0)
            //{
            //    userService.SaveSession(HttpContext, "Medicloud_organizationID", obj.organizations[0].organizationID.ToString());
            //    userService.SaveSession(HttpContext, "Medicloud_organizationName", obj.organizations[0].organizationName.ToString());

            //}
            //userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", obj.personal.subscription_expire_date.ToString());
            //userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", DateTime.Now.AddDays(10).ToString());
            return Ok();
            //}
            //else
            //{
            //    return Unauthorized();
            //}

        }



        [HttpPost("[controller]/[action]")]
        public async Task<IActionResult> LogoutAsync()
        {

            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();

            return Ok();

        }
    }
}

