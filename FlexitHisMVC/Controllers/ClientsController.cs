using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class ClientsController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private ServicePriceGroupRepository servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        public ClientsController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            servicePriceGroupRepository = new ServicePriceGroupRepository(ConnectionString);
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
    
                var response = patientCardRepo.GetPatientsByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));


                return View(response);
           
        }


        [HttpPost]
        public IActionResult AddPatient([FromBody] AddPatientDTO newPatient)
        {
      
                try
                {

                  
                    PatientRepo patientRepo = new PatientRepo(ConnectionString);


                    var newPatientID = patientRepo.InsertPatient(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), newPatient);





                    return Ok();
                }
                catch (Exception ex)
                {
                    // Handle the exception and return an appropriate response
                    return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
                }
            
         
        }






    }
}

