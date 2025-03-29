using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Abstract;
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
    public class ReceptionController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private IServicePriceGroupRepository _servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        public ReceptionController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServicePriceGroupRepository  servicePriceGroupRepository)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            _servicePriceGroupRepository = servicePriceGroupRepository;
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public IActionResult Recipe(int cardID)
        {
            if (User.Identity.IsAuthenticated)
            {
                var obj = patientCardRepo.GetUnpaidRecipe(cardID);
                return View(obj);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [HttpGet]
        public IActionResult GetPatientCards(int patientID)
        {
            if (User.Identity.IsAuthenticated)
            {
                var obj = patientCardRepo.GetUnpaidPatientCards(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), patientID);
                return Ok(obj);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }

        }
        [HttpPost]
        public IActionResult getCompanies(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                CompanyRepo select = new CompanyRepo(ConnectionString);
                var list = select.GetActiveCompanies(organizationID);
                ResponseDTO<Company> response = new ResponseDTO<Company>();
                response.data = new List<Company>();
                response.data = list;
                return Ok(response);

            }
          
                return Unauthorized();
            

        }
        [HttpPost]
        public ActionResult<NewPatientViewDTO> getPageModel(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                NewPatientViewDTO pageStruct = new NewPatientViewDTO();
                pageStruct.requestTypes = new List<RequestType>();
                pageStruct.personal = new List<User>();
                pageStruct.departments = new List<UserDepRel>();
                pageStruct.referers = new List<User>();
                pageStruct.services = new List<ServiceObj>();
                pageStruct.companies = new List<Company>();

                RequestTypeRepo requestTypeDAO = new RequestTypeRepo(ConnectionString);

                pageStruct.requestTypes.AddRange(requestTypeDAO.GetRequestType());

                ServicesRepo servicesDAO = new ServicesRepo(ConnectionString);

                pageStruct.services.AddRange(servicesDAO.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))));


                UserDepRelRepo departmentsDAO = new UserDepRelRepo(ConnectionString);

                pageStruct.departments = departmentsDAO.GetUserDepartments(1);

                UserOrganizationRel userOrganizationRel = new UserOrganizationRel(ConnectionString);


                pageStruct.personal.AddRange(userOrganizationRel.GetDoctorsByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))));


                UserRepo personalDAO = new UserRepo(ConnectionString);

                pageStruct.referers.AddRange(personalDAO.GetRefererList());

                CompanyRepo companyRepo = new CompanyRepo(ConnectionString);
                pageStruct.companies.AddRange(companyRepo.GetActiveCompanies(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))));

                pageStruct.status = 1;

                return pageStruct;
            }

            return Unauthorized();




        }
        [HttpGet]
        public ActionResult GetPriceGroupDataForCompany(int companyID)
        {
            if (User.Identity.IsAuthenticated)
            {

                var results = priceGroupCompanyRepository.GetPriceGroupDataForCompany(companyID);
                return Ok(results);

            }
            return Unauthorized();

        }

        [HttpGet]
        public ActionResult GetActiveServicesByPriceGroupID(int priceGroupID)
        {
            if (User.Identity.IsAuthenticated)
            {

                
                var results = _servicePriceGroupRepository.GetActiveServicesByPriceGroupID(priceGroupID, Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")));
                return Ok(results);

            }
            return Unauthorized();
        }

        [HttpPost]

        public IActionResult AddPatient([FromBody] PatientDTO newPatient)
        {
            if (User.Identity.IsAuthenticated)
            {

                try
                {

                    long cardID = 0;
                    PatientRepo patientRepo = new PatientRepo(ConnectionString);


                    var newPatientID = patientRepo.InsertPatient(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), newPatient);

                    if (newPatientID > 0)
                    {
                        //return BadRequest("Xəstə artıq mövcuddur");
                       
                        cardID = patientCardRepo.InsertPatientCardEnterprise(newPatient, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), newPatientID);
                        var serviceInserted = patientCardServiceRelRepo.InsertServiceToPatientCard(cardID, newPatient.serviceID, 0, newPatient.referDocID, newPatient.docID);
                        if (cardID == 0 || serviceInserted == false)
                        {
                            return BadRequest("Xəstə kartını daxil etmək mümkün olmadı.");
                        }
                    }
                    else
                    {
                       
                        cardID = patientCardRepo.InsertPatientCardEnterprise(newPatient, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), newPatient.id);
                        var serviceInserted = patientCardServiceRelRepo.InsertServiceToPatientCard(cardID, newPatient.serviceID, 0, newPatient.referDocID, newPatient.docID);
                        if (cardID == 0 || serviceInserted == false)
                        {
                            return BadRequest("Xəstə kartını daxil etmək mümkün olmadı.");
                        }

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

        [HttpPost]
        public IActionResult SearchForPatient(string fullNamePattern)
        {
            if (User.Identity.IsAuthenticated)
            {

                PatientRepo select = new PatientRepo(ConnectionString);
                var response = select.SearchForPatients(fullNamePattern, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));


                return Ok(response);

            }

            return Unauthorized();


        }

        [HttpGet]
        public IActionResult InsertServiceToPatientCard(int patientCardID, int serviceID, int depID, int senderDocID, int docID)
        {
            if (User.Identity.IsAuthenticated)
            {
                PatientCardServiceRelRepo insert = new PatientCardServiceRelRepo(ConnectionString);

                return Ok(insert.InsertServiceToPatientCard(patientCardID, serviceID, depID, senderDocID, docID));
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public IActionResult GetServices(int patientCardID)
        {
            if (User.Identity.IsAuthenticated)
            {
                PatientCardServiceRelRepo select = new PatientCardServiceRelRepo(ConnectionString);
                List<dynamic> services = select.GetServicesFromPatientCard(patientCardID, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));

                // Call the GetServicesFromPatientCard function to retrieve the list of dynamic objects
                if (services != null)
                {
                    // Return the list of dynamic objects as JSON
                    return Ok(services);
                }
                else
                {
                    // Return an error response if there was an issue retrieving the data
                    return BadRequest("Failed to retrieve services from patient card.");
                }

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet]
        public IActionResult GetDoctorsInCard(int patientCardID)
        {
            if (User.Identity.IsAuthenticated)
            {
                PatientCardServiceRelRepo select = new PatientCardServiceRelRepo(ConnectionString);
                List<dynamic> services = select.GetDoctorsFromPatientCard(patientCardID);

                // Call the GetServicesFromPatientCard function to retrieve the list of dynamic objects
                if (services != null)
                {
                    // Return the list of dynamic objects as JSON
                    return Ok(services);
                }
                else
                {
                    // Return an error response if there was an issue retrieving the data
                    return BadRequest("Failed to retrieve doctors from patient card.");
                }

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpGet]
        public IActionResult GetDoctorsInDepartment(int depID)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserDepRelRepo select = new UserDepRelRepo(ConnectionString);
                List<User> services = select.GetUsersByDepartment(depID);

                // Call the GetServicesFromPatientCard function to retrieve the list of dynamic objects
                if (services != null)
                {
                    // Return the list of dynamic objects as JSON
                    return Ok(services);
                }
                else
                {
                    // Return an error response if there was an issue retrieving the data
                    return BadRequest("Failed to retrieve services from patient card.");
                }

            }
            else
            {
                return Unauthorized();
            }

        }


        //public IActionResult SearchForPatientCard(int patientID, long organizationID)
        //{
        //    if (HttpContext.Session.GetInt32("userid") != null)
        //    {

        //        PatientCardRepo select = new PatientCardRepo(ConnectionString);
        //        var response = select.(fullNamePattern, organizationID);


        //        return Ok(response);

        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }

        //}


    }
}

