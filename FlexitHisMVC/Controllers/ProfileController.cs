using Medicloud.BLL.Service;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class ProfileController : Controller
    {
        private readonly string _connectionString;
        public IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        KassaRepo kassaRepo;
        UserService userService;
        UserRepo personalDAO;
        OrganizationService organizationService;
        public ProfileController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            kassaRepo = new KassaRepo(_connectionString);
            personalDAO = new UserRepo(_connectionString);
            organizationService = new OrganizationService(_connectionString);
            userService = new UserService(_connectionString);
        }
        [Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult getProfileInfo()
        {
            if (User.Identity.IsAuthenticated)
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
            else
            {
                return Unauthorized();
            }



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

