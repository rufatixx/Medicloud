using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.Services;
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
    //[Authorize]
    public class ServicesController : Controller
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
		RequestTypeRepo requestTypeRepo;
		private readonly IServicesService _servicesService;
		public ServicesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServicesService servicesService)
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
			requestTypeRepo = new RequestTypeRepo(ConnectionString);
			_servicesService = servicesService;
		}
		// GET: /<controller>/
		public IActionResult Index(int cardId)
		{
			if (User.Identity.IsAuthenticated)
			{


				ViewBag.services = servicesRepo.GetServicesByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));

				ViewBag.requestTypes = requestTypeRepo.GetRequestType();
				ViewBag.cardID = cardId;

				var response = patientCardServiceRelRepo.GetServicesFromPatientCard(cardId, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));

				return View(response);

			}
			else
			{
				return RedirectToAction("Index", "Login");
			}
		}






        [HttpPost]

        public async Task<IActionResult> AddService([FromBody] AddServiceDTO dto)
        {
			//if (User.Identity.IsAuthenticated)
			//{

			//    try
			//    {


			//        var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
			//        var organizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
			//        var serviceInserted = patientCardServiceRelRepo.InsertServiceToPatientCard(cardID, serviceID, 0, 0, userID);

			//        if (cardID == 0 || serviceInserted == false)
			//        {
			//            return BadRequest("Xəstə kartını daxil etmək mümkün olmadı.");
			//        }



			//        return Ok(cardID);
			//    }
			//    catch (Exception ex)
			//    {
			//        // Handle the exception and return an appropriate response
			//        return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
			//    }
			//}
			//return Unauthorized();

			int newId = await _servicesService.AddServiceAsync(dto);
			return Ok(newId);
        }


        [HttpPost]

        public IActionResult RemovePatientServiceByid(int id)
        {
            if (User.Identity.IsAuthenticated)
            {

                try
                {

                    var serviceInserted = patientCardServiceRelRepo.RemovePatientServiceById(id);


                    return Ok();
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

        public IActionResult RemoveService(long cardID, int serviceID)
        {
            if (User.Identity.IsAuthenticated)
            {

                try
                {


                    var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
                    var organizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
                    var serviceInserted = patientCardServiceRelRepo.RemoveServiceFromPatientCard(cardID, serviceID);

                    if (cardID == 0 || serviceInserted == false)
                    {
                        return BadRequest("Xəstə xidmətini silmək mümkün olmadı.");
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

		[HttpGet]
		public IActionResult GetAllServices([FromQuery] string search)
		{
            var organizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
            var response = servicesRepo.GetAllServices(organizationID,search);
			return Ok(response);
		}
	}
}

