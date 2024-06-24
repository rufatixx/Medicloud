
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
        public RegistrationController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            userService = new UserService(_connectionString);
            organizationService = new OrganizationService(_connectionString);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Step2()
        {
            return View();
        }

        public IActionResult Step3()
        {
            return View();
        }
        public IActionResult Success()
        {
            return View();
        }


        [HttpPost]
        public IActionResult SendOtpForUserRegistration(string phone)
        {

            try
            {

                var result = userService.SendOtpForUserRegistration(phone);

                if (result.Success)
                {
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
        public IActionResult CheckForOTP(string phone, string otpCode)
        {

            try
            {

                var result = userService.CheckOtpHash(phone, otpCode);

                if (result)
                {
                    return Json(new { success = true, message = "OK" });
                }
                else
                {
                    return Json(new { success = true, message = "OTP kod yalnışdır" });
                }



            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
            }



        }

        [HttpPost]
        public IActionResult AddUser(string name, string surname, string father, string phone, int specialityID, string fin, string bDate, string pwd, string organizationName)
        {
            long newUserID = userService.InsertUser(name, surname, father, specialityID, fin: fin, phone: phone, bDate: bDate, pwd: pwd);
            if (newUserID > 0)
            {
                return Ok(organizationService.AddOrganizationToNewUser(newUserID, organizationName));
            }
            else
            {
                return Ok(newUserID);//not inserted
            }
          
        }



    }
}

