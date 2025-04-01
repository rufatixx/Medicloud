using Medicloud.DAL.Repository.Abstract;
using Medicloud.DAL.Repository.PatientCard;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private IServicePriceGroupRepository _servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientRepo patientRepo;
		private readonly IPatientCardServiceRelRepository _patientCardServiceRelRepository;

		public ClientsController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServicePriceGroupRepository servicePriceGroupRepository, IPatientCardServiceRelRepository patientCardServiceRelRepository)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
			_servicePriceGroupRepository = servicePriceGroupRepository;
			patientCardRepo = new PatientCardRepo(ConnectionString);
			patientRepo = new PatientRepo(ConnectionString);
			_patientCardServiceRelRepository = patientCardServiceRelRepository;
		}

		// GET: /<controller>/
		public IActionResult Index()
        {
            
    
                var response = patientCardRepo.GetPatientsByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));


                return View(response);
           
        }


        [HttpPost]
        public IActionResult AddClient([FromBody] PatientDTO newPatient)
        {
      
                try
                {

                    var newPatientID = patientRepo.InsertPatient(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), newPatient);

                    return Ok(newPatientID);
                }
                catch (Exception ex)
                {
                    // Handle the exception and return an appropriate response
                    return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
                }
            
         
        }




        [HttpPost]
        public IActionResult UpdateClient([FromBody] PatientDTO patient)
        {

            try
            {
                var newPatientID = patientRepo.UpdatePatient(patient);

                return Ok(newPatientID);
            }
            catch (Exception ex)
            {
                // Handle the exception and return an appropriate response
                return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
            }


        }



    }
}

