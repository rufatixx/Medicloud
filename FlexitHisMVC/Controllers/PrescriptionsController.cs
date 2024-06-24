using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class PrescriptionsController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private ServicePriceGroupRepository servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        PatientDiagnoseRel patientDiagnoseRel;
        public PrescriptionsController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            servicePriceGroupRepository = new ServicePriceGroupRepository(ConnectionString);
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
            patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
        }
        // GET: /<controller>/
        public IActionResult Index(int patientID)
        {
            if (User.Identity.IsAuthenticated)
            {
             
                var response = patientCardRepo.GetAllPatientsCards(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")),patientID);
              
                return View(response);
           
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

    

      

      


    }
}

