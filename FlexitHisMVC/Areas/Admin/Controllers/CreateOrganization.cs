using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Organization;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Kassa;
using Medicloud.DAL.Repository.Role;
using Medicloud.Data;
using Medicloud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CreateOrganizationController : Controller
    {

        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        BuildingRepo buildingRepo;
        IOrganizationService _organizationService;
        IKassaRepo _kassaRepo;
        IRoleRepository _roleRepository;
        //Communications communications;
        public CreateOrganizationController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository, IOrganizationService organizationService, IKassaRepo kassaRepo)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            buildingRepo = new BuildingRepo(_connectionString);
            _organizationService = organizationService;
            _kassaRepo = kassaRepo;
            _roleRepository = roleRepository;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }

        [Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {

            return View();

        }





        [HttpPost]
        public async Task<IActionResult> CreateOrganizationAjax(string organizationName)
        {
            var userIdClaim = User.FindFirst("ID")?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Json(new { success = false, message = "İstifadəçi tapılmadı." });
            }

          
            var orgId = _organizationService.AddOrganizationToNewUser(userId, organizationName);


            if (orgId <= 0)
            {
                return Json(new { success = false, message = "Müəssisə yaradılmadı." });
            }

            var kassaId = _kassaRepo.CreateKassa($"{organizationName} - KASSA", orgId);
            if (kassaId <= 0)
            {
                return Json(new { success = false, message = "Kassa yaradılmadı." });
            }

            var linked = _kassaRepo.InsertKassaToUser(userId, kassaId, false, true);
            if (linked <= 0)
            {
                return Json(new { success = false, message = "Kassa istifadəçiyə əlavə olunmadı." });
            }
            await _roleRepository.AddUserRole(Convert.ToInt32(orgId), userId, 3);


            return Json(new
            {
                success = true,
                message = "Müəssisə uğurla yaradıldı.",
                orgId = orgId,
                orgName = organizationName
            });
        }



    }
}

