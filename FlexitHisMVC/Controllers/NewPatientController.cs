using FlexitHisMVC.Data;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.NewPatient;
using FlexitHisMVC.Models.Repository;
using FlexitHisMVC.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Controllers
{

    public class NewPatientController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private ServicePriceGroupRepository servicePriceGroupRepository;
        public NewPatientController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            servicePriceGroupRepository = new ServicePriceGroupRepository(ConnectionString);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }
        [HttpPost]
        public IActionResult getCompanies(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                CompanyRepo select = new CompanyRepo(ConnectionString);
                var list = select.GetActiveCompanies(hospitalID);
                ResponseDTO<Company> response = new ResponseDTO<Company>();
                response.data = new List<Company>();
                response.data = list;
                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        public ActionResult<NewPatientViewDTO> getPageModel(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
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

                pageStruct.services.AddRange(servicesDAO.GetServicesByHospital(hospitalID));


                UserDepRelRepo departmentsDAO = new UserDepRelRepo(ConnectionString);

                pageStruct.departments = departmentsDAO.GetUserDepartments(1);

                UserHospitalRel userHospitalRel = new UserHospitalRel(ConnectionString);
                

                pageStruct.personal.AddRange(userHospitalRel.GetUsersByHospital(hospitalID));


                UserRepo personalDAO = new UserRepo(ConnectionString);

                pageStruct.referers.AddRange(personalDAO.GetRefererList());

                CompanyRepo companyRepo = new CompanyRepo(ConnectionString);
                pageStruct.companies.AddRange(companyRepo.GetActiveCompanies(hospitalID));

                pageStruct.status = 1;

                return pageStruct;
            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpGet]
        public ActionResult GetPriceGroupDataForCompany(int companyID)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return Unauthorized();
            }
            var results = priceGroupCompanyRepository.GetPriceGroupDataForCompany(companyID);
            return Ok(results);
        }

        [HttpGet]
        public ActionResult GetActiveServicesByPriceGroupID(int priceGroupID)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return Unauthorized();
            }
            var results = servicePriceGroupRepository.GetActiveServicesByPriceGroupID(priceGroupID);
            return Ok(results);
        }

        [HttpPost]

        public IActionResult AddPatient([FromBody] AddPatientDTO newPatient)
        {
            if (HttpContext.Session.GetInt32("userid") == null)
            {
                return Unauthorized();
            }

            try
            {
                long patientID;
                long cardID = 0;
                PatientRepo patientRepo = new PatientRepo(ConnectionString);

                if (newPatient.foundPatientID > 0)
                {
                    patientID = newPatient.foundPatientID;
                }
                else
                {
                    patientID = patientRepo.InsertPatient(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), newPatient);

                    if (patientID == 0)
                    {
                        return BadRequest("Xəstəni artıq mövcuddur");
                    }
                }

                PatientCardRepo patientCardRepo = new PatientCardRepo(ConnectionString);
                PatientCardServiceRelRepo patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
                cardID = patientCardRepo.InsertPatientCard(newPatient, Convert.ToInt32(HttpContext.Session.GetInt32("userid")), patientID);
                var serviceInserted = patientCardServiceRelRepo.InsertServiceToPatientCard(cardID, newPatient.serviceID, 0, newPatient.referDocID, newPatient.docID);
                if (cardID == 0 || serviceInserted ==false)
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

        [HttpPost]
        public IActionResult SearchForPatient(string fullNamePattern, long hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {

                PatientRepo select = new PatientRepo(ConnectionString);
                var response = select.SearchForPatients(fullNamePattern, hospitalID);


                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpGet]
        public IActionResult InsertServiceToPatientCard(int patientCardID, int serviceID,int depID, int senderDocID, int docID) {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PatientCardServiceRelRepo insert = new PatientCardServiceRelRepo(ConnectionString);
               
                return Ok(insert.InsertServiceToPatientCard(patientCardID, serviceID,depID, senderDocID, docID));
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public IActionResult GetServices(int patientCardID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PatientCardServiceRelRepo select = new PatientCardServiceRelRepo(ConnectionString);
                List<dynamic> services = select.GetServicesFromPatientCard(patientCardID);

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
            if (HttpContext.Session.GetInt32("userid") != null)
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
            if (HttpContext.Session.GetInt32("userid") != null)
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


        //public IActionResult SearchForPatientCard(int patientID, long hospitalID)
        //{
        //    if (HttpContext.Session.GetInt32("userid") != null)
        //    {

        //        PatientCardRepo select = new PatientCardRepo(ConnectionString);
        //        var response = select.(fullNamePattern, hospitalID);


        //        return Ok(response);

        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }

        //}


    }
}

