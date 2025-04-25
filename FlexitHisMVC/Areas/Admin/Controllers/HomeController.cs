using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Organization;
using Medicloud.BLL.Services;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Kassa;
using Medicloud.DAL.Repository.Plan;
using Medicloud.Data;
using Medicloud.Models.DTO;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509.SigI;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly string _connectionString;
        public IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        IKassaRepo _kassaRepo;
        IUserService _userService;
        IPlanRepository _planRepository;
        IOrganizationService _organizationService;
        private ServicesRepo _servicesRepo;
        private PatientRepo _patientRepo;
        private PatientCardRepo _patientCardRepo;
        public HomeController(IConfiguration configuration, IPlanRepository planRepository, IOrganizationService organizationService, IWebHostEnvironment hostingEnvironment, IUserService userService, IKassaRepo kassaRepo)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            _kassaRepo = kassaRepo;
            _userService = userService;
            _organizationService = organizationService;
            _servicesRepo = new ServicesRepo(_connectionString);
            _planRepository = planRepository;
            _patientRepo = new PatientRepo(_connectionString);
            _patientCardRepo = new PatientCardRepo(_connectionString);

        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var userID = User.FindFirst("ID")?.Value;

            //ViewBag.expiredDate = _planRepository.GetUserPlanByUserId(Convert.ToInt32(userID)).expire_date;
            DateTime.TryParse(HttpContext.Session.GetString("Medicloud_UserPlanExpireDate"), out var planExpiryDate);
            ViewBag.expiredDate = planExpiryDate;
            ViewBag.SelectedOrganization = HttpContext.Session.GetString("Medicloud_organizationName");
            ViewBag.patientStatisticsDTO = _patientRepo.GetPatientStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            ViewBag.patientCardStatisticsDTO = _patientCardRepo.GetPatientCardStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            ViewBag.imagePath = "/res/admin/company.png";

            ViewBag.services = _servicesRepo.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            return View();


        }

    }
}

