
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
            if (HttpContext.Session.GetString("recoveryPhone") != null || HttpContext.Session.GetString("recoveryEmail") != null)
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
            if (HttpContext.Session.GetString("recoveryPhone") != null || HttpContext.Session.GetString("recoveryEmail") != null)
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
            if (HttpContext.Session.GetString("recoveryPhone") != null || HttpContext.Session.GetString("recoveryEmail") != null)
            {
                HttpContext.Session.Remove("recoveryOtpCode");
                HttpContext.Session.Remove("recoveryPhone");
                HttpContext.Session.Remove("recoveryEmail");
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Recovery");
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendRecoveryOtpForUser(string content,int type)
        {
            if (type==0)
            {
                throw new Exception();
            }
            try
            {

                var result = await userService.SendRecoveryOtpForUser(content,type);
                if (result.Success)
                {
                    if (type==1)
                    {
                        HttpContext.Session.SetString("recoveryPhone", content);
                    }
                    else if (type==2)
                    {
                        HttpContext.Session.SetString("recoveryEmail", content);

                    }
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
                var email = HttpContext.Session.GetString("recoveryEmail");
                bool result = false;
                if (!string.IsNullOrEmpty(phone))
                {
                    result = userService.CheckRecoveryOtpHash(phone, otpCode, 1);
                }
                else if (!string.IsNullOrEmpty(email))
                {
                    result = userService.CheckRecoveryOtpHash(email, otpCode, 2);

                }

                //if (result)
                //    var phone = HttpContext.Session.GetString("recoveryPhone");
                //var result = userService.CheckRecoveryOtpHash(phone, otpCode);

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
                //var phone = HttpContext.Session.GetString("recoveryPhone");
                var phone = HttpContext.Session.GetString("recoveryPhone");
                var email = HttpContext.Session.GetString("recoveryEmail");
                if (!string.IsNullOrEmpty(phone))
                {
                    userService.UpdatePassword(otpCode, phone, password,1);

                }
                else if (!string.IsNullOrEmpty(email))
                {
                    userService.UpdatePassword(otpCode, email, password,2);

                }


                return Ok();



            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }




        }



    }
}

