
using Medicloud.BLL.Service;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    public class RecoveryController : Controller
    {
        private readonly string _connectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        UserService userService;
        OrganizationService organizationService;
        private readonly SpecialityService _specialityService;
        public RecoveryController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, SpecialityService specialityService)
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
            if (HttpContext.Session.GetString("recoveryPhone") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Recovery");
            }
        }

        public IActionResult Step3()
        {
            if (HttpContext.Session.GetString("recoveryPhone") != null)
            {

               

                return View();
            }
            else
            {
                return RedirectToAction("Index", "Recovery");
            }
        }

        public IActionResult Success()
        {
            if (HttpContext.Session.GetString("recoveryPhone") != null)
            {
                HttpContext.Session.Remove("recoveryOtpCode");
                HttpContext.Session.Remove("recoveryPhone");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Recovery");
            }
        }


        [HttpPost]
        public IActionResult SendRecoveryOtpForUser(string phone)
        {

            try
            {

                var result = userService.SendRecoveryOtpForUser(phone);
                if (result.Success)
                {
                    HttpContext.Session.SetString("recoveryPhone", phone);
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
        public IActionResult CheckForRecoveryOTP(string otpCode)
        {

            try
            {
                var phone = HttpContext.Session.GetString("recoveryPhone");
                var result = userService.CheckRecoveryOtpHash(phone, otpCode);

                if (result)
                {
                    HttpContext.Session.SetString("recoveryOtpCode", otpCode);
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
        public IActionResult UpdatePass(string password)
        {
            try
            {
                var otpCode = HttpContext.Session.GetString("recoveryOtpCode");
                var phone = HttpContext.Session.GetString("recoveryPhone");
                userService.UpdatePassword(otpCode, phone, password);


                return Ok();



            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }




        }



    }
}

