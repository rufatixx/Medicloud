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
        ServicesRepo servicesRepo;
        PatientRepo patientRepo;
        RequestTypeRepo requestTypeDAO;
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
            servicesRepo = new ServicesRepo(ConnectionString);
            patientRepo = new PatientRepo(ConnectionString);
           requestTypeDAO = new RequestTypeRepo(ConnectionString);

        }
        // GET: /<controller>/
        public IActionResult Index(int patientID)
        {
            if (User.Identity.IsAuthenticated)
            {


             ViewBag.services =  servicesRepo.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));

                ViewBag.requestTypes = requestTypeDAO.GetRequestType();


                var response = patientCardRepo.GetAllPatientsCards(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")),patientID);
              
                return View(response);
           
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }



        [HttpPost]

        public IActionResult AddPrescription(int requestTypeID,long patientID,long cardID, int serviceID, string note)
        {
            if (User.Identity.IsAuthenticated)
            {

                try
                {


                    var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
                    var organizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));

                    if (cardID==0)
                    {
                        cardID = patientCardRepo.CreatePatientCard(requestTypeID, userID, patientID, organizationID, serviceID,note:note);
                    }

                       
                        var serviceInserted = patientCardServiceRelRepo.InsertServiceToPatientCard(cardID, serviceID, 0,0, userID);
                        if (cardID == 0 || serviceInserted == false)
                        {
                            return BadRequest("Xəstə kartını daxil etmək mümkün olmadı.");
                        }

                   

                    return Ok(cardID);
                }
                catch (Exception ex)
                {
                    // Handle the exception and return an appropriate response
                    return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
                }
            }
            return Unauthorized();
        }





    }
}

