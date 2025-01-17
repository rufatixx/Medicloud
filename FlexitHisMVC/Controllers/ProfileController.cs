using Medicloud.BLL.Service;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Medicloud.Models.Repository;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly string _connectionString;
        public IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        KassaRepo kassaRepo;
        UserService userService;
        UserRepo personalDAO;
		PlanRepository planRepository;
        OrganizationService organizationService;
        private ServicesRepo servicesRepo;
        public ProfileController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            kassaRepo = new KassaRepo(_connectionString);
            personalDAO = new UserRepo(_connectionString);
            organizationService = new OrganizationService(_connectionString);
            userService = new UserService(_connectionString);
            servicesRepo = new ServicesRepo(_connectionString);
			planRepository = new PlanRepository(_connectionString);

        }
  
        // GET: /<controller>/
        public IActionResult Index()
        {
            var userID = User.FindFirst("ID")?.Value;

			ViewBag.expiredDate = planRepository.GetUserPlanByUserId(Convert.ToInt32(userID)).expire_date;
			var user = personalDAO.GetUserByID(Convert.ToInt32(userID));
			if (!string.IsNullOrEmpty(user.imagePath))
			{
				string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.imagePath.TrimStart('/'));
				if (!System.IO.File.Exists(path))
				{
					user.imagePath = "";
				}
			}
			ViewBag.userData =user;

            ViewBag.services = servicesRepo.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));

			var vm = new ProfileViewModel
			{
				UserSurname = user.name,
				UserName = user.surname,
				Email = user.email,
				ImagePath = user.imagePath,
				SpecialityName = user.speciality?.name ?? "",
				PhoneNumber = user.mobile
			};
            return View(vm);
        }

        [HttpPost]
        public IActionResult InsertService([FromBody] ServiceObj serviceObj)
        {

            if (User.Identity.IsAuthenticated)
            {
                serviceObj.organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
                var result = servicesRepo.InsertService(serviceObj);
                if (result > 0)
                {
                    return Ok();
                }
                else if (result == -1)
                {
                    // A matching record already exists
                    // Return 409 Conflict status with an error message
                    return StatusCode(409, "Xidmət artıq mövcuddur");

                }
                else
                {
                    return BadRequest("Xəta baş verdi"); // Return HTTP 400 Bad Request if the update failed
                }
            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        public IActionResult UpdateService([FromBody] ServiceObj service)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Update the service in the repository or database
                int updateResult = servicesRepo.UpdateService(service);

                if (updateResult > 0)
                {
                    return Ok(); // Return HTTP 200 OK if the update was successful
                }
                else if (updateResult == -1)
                {
                    // A matching record already exists
                    // Return 409 Conflict status with an error message
                    return StatusCode(409, "Xidmət artıq mövcuddur");

                }
                else
                {
                    return BadRequest("Xəta baş verdi"); // Return HTTP 400 Bad Request if the update failed
                }

            }

            return Unauthorized();


        }

        public IActionResult getProfileInfo()
        {
        
                var userID = Request.Cookies["Medicloud_userID"];

                //long formattedPhone = regexPhone(phone);
                UserDTO status = new UserDTO();
                status.personal = new User();
                status.organizations = new List<Organization>();
                status.kassaList = new List<Kassa>();

                try
                {
                  
                    status.personal = personalDAO.GetUserByID(Convert.ToInt32(userID));

                   
                    status.organizations = organizationService.GetOrganizationsByUser(status.personal.ID);


                    status.kassaList = kassaRepo.GetUserAllowedKassaList(status.personal.ID);


                }
                catch (Exception ex)
                {

                    Medicloud.StandardMessages.CallSerilog(ex);
                    Console.WriteLine($"Exception: {ex.Message}");
                    // status.responseString = $"Exception: {ex.Message}";
                }
                return Ok(status);

         



        }

        [HttpPost]
        public IActionResult UpdateUser(int userID, string name, string surname, string father, int specialityID, string passportSerialNum, string fin, string mobile, string email, string bDate, string username, int isUser, int isDr, int isActive, int isAdmin)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserRepo user = new UserRepo(_connectionString);

                return Ok(user.UpdateUser(userID, name, surname, father, specialityID, passportSerialNum, fin, mobile, email, bDate, username, isUser, isDr, isActive, isAdmin));

            }

            return Unauthorized();




        }

        [HttpGet]
        // Сохранение данных пользователя в куки
        public IActionResult SaveUserSetting(string key, string value)
        {
            if (User.Identity.IsAuthenticated)
            {
                userService.SaveSession(HttpContext, key, value);

                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

    }
}

