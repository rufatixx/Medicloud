using Medicloud.BLL.Models;
using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Organization;
using Medicloud.BLL.Services;
using Medicloud.BLL.Services.WorkHour;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Kassa;
using Medicloud.DAL.Repository.Plan;
using Medicloud.DAL.Repository.Role;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly string _connectionString;
        public IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        IKassaRepo _kassaRepo;

        IUserService _userService;
        IPlanRepository _planRepository;
        IOrganizationService _organizationService;
        private ServicesRepo servicesRepo;
        private readonly IWorkHourService _workHourService;
        private readonly IRoleRepository _roleRepository;
        public ProfileController(IConfiguration configuration, IPlanRepository planRepository, IOrganizationService organizationService, IWebHostEnvironment hostingEnvironment, IUserService userService, IKassaRepo kassaRepo, IWorkHourService workHourService, IRoleRepository roleRepository)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            _kassaRepo = kassaRepo;

            _organizationService = organizationService;
            _userService = userService;
            servicesRepo = new ServicesRepo(_connectionString);
            _planRepository = planRepository;
            _workHourService = workHourService;
            _roleRepository=roleRepository;
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            var userID = User.FindFirst("ID")?.Value;

            ViewBag.expiredDate = _planRepository.GetUserPlanByUserId(Convert.ToInt32(userID)).expire_date;
            var user = _userService.GetUserById(Convert.ToInt32(userID));
            if (!string.IsNullOrEmpty(user.imagePath))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.imagePath.TrimStart('/'));
                if (!System.IO.File.Exists(path))
                {
                    user.imagePath = "";
                }
            }
            ViewBag.userData = user;

            ViewBag.services = servicesRepo.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            return View();
        }


        public async Task<IActionResult> Edit()
        {
            var userID = User.FindFirst("ID")?.Value;
            var plan = _planRepository.GetUserPlanByUserId(Convert.ToInt32(userID));
            if (plan != null)
            {
                ViewBag.expiredDate = plan.expire_date;

            }
            var user = await _userService.GetOnlyUserById(Convert.ToInt32(userID));
            if (!string.IsNullOrEmpty(user.imagePath))
            {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.imagePath.TrimStart('/'));
                if (!System.IO.File.Exists(path))
                {
                    user.imagePath = "";
                }
            }
            //ViewBag.userData =user;

            ViewBag.services = servicesRepo.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromForm] string name, string surname, string father, string fin, string passportSerialNum)
        {
            //Console.WriteLine($"emailll {email}");

            var userID = int.Parse(User.FindFirst("ID")?.Value);

            long updated = _userService.UpdateUser(userID, name: name, surname: surname, father: father, fin: fin, passportSerialNum: passportSerialNum);
            return RedirectToAction("Edit");
        }

        public async Task<IActionResult> UpdateProfileImage([FromForm] IFormFile profilePicture, string existingPhotoPath)
        {
            string relativeFilePath = "";

            if (!string.IsNullOrEmpty(existingPhotoPath))
            {
                string existingPhotoFullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingPhotoPath.TrimStart('/'));

                // Check if the file exists before deleting it
                if (System.IO.File.Exists(existingPhotoFullPath))
                {
                    // Delete the existing photo
                    System.IO.File.Delete(existingPhotoFullPath);
                }
            }

            if (profilePicture != null && profilePicture.Length > 0)
            {
                string fileExtension = Path.GetExtension(profilePicture.FileName);

                string fileName = Guid.NewGuid().ToString() + fileExtension;
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "user_images", fileName);
                relativeFilePath = "/user_images/" + fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profilePicture.CopyToAsync(stream);
                }
            }

            var userID = int.Parse(User.FindFirst("ID")?.Value);

            long updated = _userService.UpdateUser(userID, imagePath: relativeFilePath);
            return RedirectToAction("Edit");
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
            status.personal = new UserDAO();
            status.organizations = new List<OrganizationDAO>();
            status.kassaList = new List<KassaDAO>();

            try
            {

                status.personal = _userService.GetUserById(Convert.ToInt32(userID));


                status.organizations = _organizationService.GetOrganizationsByUser(status.personal.ID);


                status.kassaList = _kassaRepo.GetUserAllowedKassaList(status.personal.ID);


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


                return Ok(_userService.UpdateUser(userID, name, surname, father, specialityID, passportSerialNum, fin, mobile, email, bDate, username, isUser, isDr, isActive, isAdmin));

            }

            return Unauthorized();




        }

        [HttpGet]
        // Сохранение данных пользователя в куки
        public IActionResult SaveUserSetting(string key, string value)
        {
            if (User.Identity.IsAuthenticated)
            {
                _userService.SaveSession(HttpContext, key, value);

                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        public async Task<IActionResult> WorkHours(int userId = 0)
        {
            var vm = new WorkHourViewModel();
            int currentUserId = int.Parse(User.FindFirst("ID")?.Value);
            int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            bool hasAccess = false;
            if (userId == 0 || userId== currentUserId)
            {
                userId = currentUserId;
                hasAccess = true;
                vm.IsOwnerUser=true;
            }
            else
            {
                var userOrganizations = _organizationService.GetOrganizationsByUser(userId);
                if (userOrganizations != null && userOrganizations.Any(o => o.organizationID==organizationId))
                {
                    var userRoles = await _roleRepository.GetUserRoles(organizationId, currentUserId);
                    hasAccess=userRoles.Any(ur => ur.id==3 || ur.id==7);
                    if (hasAccess)
                    {
                        var user = await _userService.GetOnlyUserById(userId);
                        vm.UserFullName = $"{user?.name} {user?.surname}";
                    }


                }

            }

            if (hasAccess)
            {
                var workHours = await _workHourService.GetOrganizationUserWorkHours(userId, organizationId);
                vm.WorkHours = workHours;
            }


 
            //if(workHours != null)
            //{
            //	foreach (var item in workHours)
            //	{
            //		if (item.Breaks == null) item.Breaks = new();
            //		Console.WriteLine(item?.startTime);
            //	}
            //}




            return View(vm);

        }


        [HttpPost]
        public async Task<IActionResult> UpdateWorkHoursWithForm(WorkHourViewModel vm)
        {
            if (vm == null)
            {
                return RedirectToAction("Index");
            }
            Console.WriteLine(vm.OpenedDays);
            Console.WriteLine(vm.ClosedDays);
            var openedDaysList = JsonConvert.DeserializeObject<List<int>>(vm.OpenedDays);
            var closedDaysList = JsonConvert.DeserializeObject<List<int>>(vm.ClosedDays);
            var dto = new UpdateWorkHourDTO
            {
                ClosedDays = closedDaysList,
                OpenedDays = openedDaysList
            };
            var updated = await _workHourService.UpdateWorkHours(dto);

            return RedirectToAction("Step9", new { organizationId = vm.id });

        }


        [HttpPost]
        public async Task<IActionResult> UpdateWorkHours([FromBody] UpdateWorkHourDTO dto)
        {
            //int newId = await _staffService.UpdateStaffWorkHours(dto);
            await _workHourService.UpdateWorkHours(dto);
            return Ok();
        }
    }
}

