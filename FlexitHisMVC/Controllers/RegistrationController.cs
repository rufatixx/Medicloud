
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.User;
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
		private readonly INUserService _userService;
		public RegistrationController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, SpecialityService specialityService, INUserService nuserService)
		{
			Configuration = configuration;
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			userService = new UserService(_connectionString);
			organizationService = new OrganizationService(_connectionString);
			_specialityService = specialityService;
			_userService = nuserService;
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
        public async Task<IActionResult> AddUser(IFormFile profileImage, string name, string surname, string father, int specialityID, string fin, string bDate, string pwd)
        {


			var otpCode = HttpContext.Session.GetString("registrationOtpCode");
            var phone = HttpContext.Session.GetString("registrationPhone");
			string relativeFilePath = "";
            if (userService.CheckOtpHash(phone, otpCode))
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


				//var newUserID = userService.AddUser(phone, name, surname, father, specialityID, fin: fin, bDate: bDate, pwd: pwd, "", 4, relativeFilePath);
				int userId = await _userService.UpdateUserAsync(phone, new()
				{
					name = name,
					surname = surname,
					father = father,
					specialityID = specialityID,
					fin = fin,
					bDate = bDate,
					pwd = pwd

				});
				if (userId>0)
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

