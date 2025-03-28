using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medicloud.BLL.Service;
using Medicloud.DAL.Repository;
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
        KassaRepo kassaRepo;
        UserService userService;
        UserRepo personalDAO;
        PlanRepository planRepository;
        OrganizationService organizationService;
        private ServicesRepo servicesRepo;
        private PatientRepo _patientRepo;
        private PatientCardRepo _patientCardRepo;
        public HomeController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
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
            _patientRepo = new PatientRepo(_connectionString);
            _patientCardRepo = new PatientCardRepo(_connectionString);

        }


        // GET: /<controller>/
        public IActionResult Index()
        {
            var userID = User.FindFirst("ID")?.Value;

            ViewBag.expiredDate = planRepository.GetUserPlanByUserId(Convert.ToInt32(userID)).expire_date;
            ViewBag.SelectedOrganization = HttpContext.Session.GetString("Medicloud_organizationName");
            ViewBag.patientStatisticsDTO = _patientRepo.GetPatientStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            ViewBag.patientCardStatisticsDTO = _patientCardRepo.GetPatientCardStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            ViewBag.imagePath = "/res/admin/company.png";

            ViewBag.services = servicesRepo.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
            return View();


        }

    }
}

