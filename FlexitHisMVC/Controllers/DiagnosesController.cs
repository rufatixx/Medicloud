using Medicloud.DAL.Repository.Abstract;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    [Authorize]
    public class DiagnosesController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private IServicePriceGroupRepository _servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        PatientDiagnoseRel patientDiagnoseRel;
        public DiagnosesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServicePriceGroupRepository servicePriceGroupRepository)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            _servicePriceGroupRepository = servicePriceGroupRepository;
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
            patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
        }
        // GET: /<controller>/
        public IActionResult Index(string patientFullName,int patientID)
        {
            if (User.Identity.IsAuthenticated)
            {
               
                var response = patientDiagnoseRel.GetPatientToDiagnose(patientID);
                ViewBag.patientFullName = patientFullName;
                return View(response);

            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

    

      

      


    }
}

