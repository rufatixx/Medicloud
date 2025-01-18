
using Medicloud.BLL.Service;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Microsoft.AspNetCore.Authorization;
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
            if (HttpContext.Session.GetString("registrationPhone") != null || HttpContext.Session.GetString("registrationEmail") != null)
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
            else if (HttpContext.Session.GetString("registrationEmail") != null)
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
            if (HttpContext.Session.GetString("registrationPhone") != null || HttpContext.Session.GetString("registrationEmail") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Registration");
            }
        }


        [HttpPost]
        public async Task<IActionResult> SendOtpForUserRegistration(string content,int type)
        {

            try
            {
                if (type==0)
                {
                    throw new Exception();
                }
                var result =await userService.SendOtpForUserRegistration(content,type);

                if (result.Success)
                {
                    if (type==1)
                    {
                        HttpContext.Session.SetString("registrationPhone", content);
                    }
                    else if (type==2)
                    {
                        HttpContext.Session.SetString("registrationEmail", content);

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
        public IActionResult CheckForOTP(string otpCode)
        {

            try
            {
                var phone = HttpContext.Session.GetString("registrationPhone");
                var email = HttpContext.Session.GetString("registrationEmail");
                bool result = false;
                if(!string.IsNullOrEmpty(phone))
                {
                    result = userService.CheckOtpHash(phone, otpCode,1);
                }
                else if(!string.IsNullOrEmpty(email))
                {
                    result = userService.CheckOtpHash(email, otpCode, 2);

                }

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
        public async Task<IActionResult> AddUser(IFormFile profileImage, string name, string surname, string father, int specialityID, string fin, string bDate, string pwd, string organizationName)
        {


			var otpCode = HttpContext.Session.GetString("registrationOtpCode");
            var phone = HttpContext.Session.GetString("registrationPhone");
            var email = HttpContext.Session.GetString("registrationEmail");
			string relativeFilePath = "";

            bool result = false;
            if (!string.IsNullOrEmpty(phone))
            {
                result = userService.CheckOtpHash(phone, otpCode, 1);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                result = userService.CheckOtpHash(email, otpCode, 2);

            }
            if (result)
            {
				if (profileImage != null && profileImage.Length > 0)
				{
					string fileExtension = Path.GetExtension(profileImage.FileName);

					string fileName = Guid.NewGuid().ToString() + fileExtension;
					var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "user_images", fileName);
					relativeFilePath = "/user_images/" + fileName;
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await profileImage.CopyToAsync(stream);
					}
				}


				var newUserID = userService.AddUser(phone,email, name, surname, father, specialityID, fin: fin, bDate: bDate, pwd: pwd, organizationName, 4,relativeFilePath);
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

