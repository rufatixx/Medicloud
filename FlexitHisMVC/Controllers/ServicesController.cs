using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class ServicesController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private ServicePriceGroupRepository servicePriceGroupRepository;
        private ServicesRepo serviceRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        PatientDiagnoseRel patientDiagnoseRel;
        public ServicesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            servicePriceGroupRepository = new ServicePriceGroupRepository(ConnectionString);
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
            patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
            serviceRepository = new ServicesRepo(ConnectionString);
        }
        // GET: /<controller>/
        public IActionResult Index(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
             
                var response = patientCardServiceRelRepo.GetServicesFromPatientCard(id,Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
              
                return View(response);
           
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }


        [HttpGet]
        public IActionResult GetAllServices([FromQuery] string search)
        {
            var response = serviceRepository.GetAllServices(search);
            return Ok(response);
        }
    }
}

