
using Medicloud.BLL.Service;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly string _connectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        UserService userService;
        OrganizationService organizationService;
        private readonly SpecialityService _specialityService;
        public RegistrationController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, SpecialityService specialityService)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            userService = new UserService(_connectionString);
            organizationService = new OrganizationService(_connectionString);
            _specialityService = specialityService;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Step2()
        {
            if (HttpContext.Session.GetString("registrationPhone") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Registration");
            }
        }

        public IActionResult Step3()
        {
            if (HttpContext.Session.GetString("registrationPhone") != null)
            {
                ViewBag.specialities = _specialityService.GetSpecialities();
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Registration");
            }
        }

        public IActionResult Success()
        {
            if (HttpContext.Session.GetString("registrationPhone") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Registration");
            }
        }


        [HttpPost]
        public IActionResult SendOtpForUserRegistration(string phone)
        {

            try
            {

                var result = userService.SendOtpForUserRegistration(phone);

                if (result.Success)
                {
                    HttpContext.Session.SetString("registrationPhone", phone);
                    return Json(new { success = true, message = result.Message });
                }
                else
                {
                    return Json(new { success = false, message = result.Message });
                }



            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
            }



        }

        [HttpPost]
        public IActionResult CheckForOTP(string otpCode)
        {

            try
            {
                var phone = HttpContext.Session.GetString("registrationPhone");
                var result = userService.CheckOtpHash(phone, otpCode);

                if (result)
                {
                    HttpContext.Session.SetString("registrationOtpCode", otpCode);
                    return Json(new { success = true, message = "OK" });
                }
                else
                {
                    return Json(new { success = false, message = "OTP kod yalnışdır" });
                }



            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
            }



        }

        [HttpPost]
        public IActionResult AddUser(string name, string surname, string father, int specialityID, string fin, string bDate, string pwd, string organizationName)
        {
            var otpCode = HttpContext.Session.GetString("registrationOtpCode");
            var phone = HttpContext.Session.GetString("registrationPhone");
            if (userService.CheckOtpHash(phone, otpCode))
            {
                var newUserID = userService.AddUser(phone, name, surname, father, specialityID, fin: fin, bDate: bDate, pwd: pwd, organizationName,4);
                if (newUserID)
                {
                    HttpContext.Session.Remove("recoveryOtpCode");
                    HttpContext.Session.Remove("recoveryPhone");

                    return Ok();
                }
               
            }
         
            return BadRequest();

        }



    }
}

