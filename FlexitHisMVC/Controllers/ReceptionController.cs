using Medicloud.BLL.Services;
using Medicloud.BLL.Services.Abstract;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.DAL.Repository.Patient;
using Medicloud.DAL.Repository.PatientCard;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Medicloud.ViewModels;
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
        IUserService _userService;
		private readonly IPatientCardServiceRelRepository _patientCardServiceRelRepository;
		private readonly IPatientCardRepository _patientCardRepository;
		private readonly IPatientRepository _patientRepository;
		private readonly DepartmentsRepo _departmentsRepo;
		private readonly IPatientCardService _patientCardService;

		public ReceptionController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IUserService userService, IServicePriceGroupRepository servicePriceGroupRepository, IPatientCardServiceRelRepository patientCardServiceRelRepository, IPatientCardRepository patientCardRepository, IPatientRepository patientRepository, IPatientCardService patientCardService)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
			_servicePriceGroupRepository = servicePriceGroupRepository;
			patientCardRepo = new PatientCardRepo(ConnectionString);

			_userService = userService;
			_patientCardServiceRelRepository = patientCardServiceRelRepository;
			_patientCardRepository = patientCardRepository;
			_patientRepository = patientRepository;
			_departmentsRepo = new DepartmentsRepo(ConnectionString);
			_patientCardService = patientCardService;
		}
		// GET: /<controller>/
		public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
				int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

				var departments = _departmentsRepo.GetDepartmentsByOrganization(organizationId);
				var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
				var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
				var doctors = await _userService.GetDoctorUsersByOrganization(organizationID);


				var vm = new ReceptionViewModel
				{
					Departments = departments,
					Doctors=doctors,
				};
                return View(vm);
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
                var obj = patientCardRepo.GetRecipe(cardID);
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
                organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
                CompanyRepo select = new CompanyRepo(ConnectionString);
                var list = select.GetActiveCompanies(organizationID);
                ResponseDTO<CompanyDAO> response = new ResponseDTO<CompanyDAO>();
                response.data = new List<CompanyDAO>();
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
                pageStruct.personal = new List<UserDAO>();
                pageStruct.departments = new List<UserDepRel>();
                pageStruct.referers = new List<UserDAO>();
                pageStruct.services = new List<ServiceObj>();
                pageStruct.companies = new List<CompanyDAO>();

				int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));


				ServicesRepo servicesDAO = new ServicesRepo(ConnectionString);

                pageStruct.services.AddRange(servicesDAO.GetServicesByOrganization(organizationId));


                //UserDepRelRepo departmentsDAO = new UserDepRelRepo(ConnectionString);
                DepartmentsRepo departmentsDAO = new DepartmentsRepo(ConnectionString);

                //pageStruct.departments = departmentsDAO.GetDepartmentsByOrganization(organizationId);

                UserOrganizationRel userOrganizationRel = new UserOrganizationRel(ConnectionString);


                //pageStruct.personal.AddRange(userOrganizationRel.GetDoctorsByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))));


             

                //pageStruct.referers.AddRange(_userService.GetRefererList());

                CompanyRepo companyRepo = new CompanyRepo(ConnectionString);
                //pageStruct.companies.AddRange(companyRepo.GetActiveCompanies(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))));

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
                Console.WriteLine($"cmpid {companyID}");
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

        public async Task<IActionResult> AddPatient([FromBody] PatientDTO newPatient)
        {

			if(newPatient == null)
			{
				Console.WriteLine("NEWPATIENT NULL");
			}
            if (User.Identity.IsAuthenticated)
            {
				int userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
				int organzationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

				try
                {

                    long cardID = 0;
					var pCardDAO = new PatientCardDAO
					{
						serviceID = newPatient.serviceID,
						departmentID = newPatient.depID,
						docID = newPatient.docID,
						note = newPatient.note,
						priceGroupID = newPatient.priceGroupID,
						organizationID = organzationId,
						userID = userId,
						referDocID = (int)newPatient.referDocID,
						requestTypeID = newPatient.requestTypeID,
						startDate =newPatient.selectedDate.Date + newPatient.startTime,
                        endDate =newPatient.selectedDate.Date + newPatient.endTime,
						isOnline=newPatient.isOnline,
						companyID = newPatient.companyID,
					};
					if(newPatient.id>0)
					{
						pCardDAO.patientID = newPatient.id;
					}
					else
					{

						pCardDAO.patientID = await _patientRepository.AddAsync(new()
						{
							name = newPatient.name,
							surname = newPatient.surname,
							clientEmail = newPatient.clientEmail,
							bDate = newPatient.birthDate,
							clientPhone = newPatient.clientPhone,
							father = newPatient.father,
							fin = newPatient.fin,
							genderID = newPatient.genderID,
							organizationID = organzationId,
							userID = userId,
							orgReasonId=newPatient.orgReasonId,
						});

					}
					cardID = await _patientCardRepository.AddAsync(pCardDAO);
					var serviceInserted = await _patientCardServiceRelRepository.InsertServiceToPatientCard(cardID, newPatient.serviceID, 0, newPatient.referDocID, newPatient.docID,newPatient.priceGroupID);
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
		public async Task<IActionResult> GetCardById(int cardId)
		{
			if (User.Identity.IsAuthenticated)
			{
				var response=await _patientCardService.GetPatientCardById(cardId);
				Console.WriteLine(response.companyID);
				return Ok(response);

			}

			return Unauthorized();


		}

		[HttpGet]
        public async Task<IActionResult> InsertServiceToPatientCard(int patientCardID, int serviceID, int depID, int senderDocID, int docID,int priceGroupId)
        {
            if (User.Identity.IsAuthenticated)
            {
				var result = await _patientCardServiceRelRepository.InsertServiceToPatientCard(patientCardID, serviceID, depID, senderDocID, docID, priceGroupId);

				return Ok(result);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetServices(int patientCardID)
        {
            if (User.Identity.IsAuthenticated)
            {
                //PatientCardServiceRelRepo select = new PatientCardServiceRelRepo(ConnectionString);
                //List<dynamic> services = select.GetServicesFromPatientCard(patientCardID, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
				var services= await _patientCardServiceRelRepository.GetServicesFromPatientCard(patientCardID, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
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
        public async Task<IActionResult> GetDoctorsInCard(int patientCardID)
        {
            if (User.Identity.IsAuthenticated)
            {
                //PatientCardServiceRelRepo select = new PatientCardServiceRelRepo(ConnectionString);
                //List<dynamic> services = select.GetDoctorsFromPatientCard(patientCardID);

				var services=await _patientCardServiceRelRepository.GetDoctorsFromPatientCard(patientCardID);

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
                List<UserDAO> services = select.GetUsersByDepartment(depID);

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

