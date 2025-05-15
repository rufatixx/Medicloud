using Medicloud.BLL.Models;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.Abstract;
using Medicloud.BLL.Services.Patient;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.DAL.Repository.PatientCard;
using Medicloud.DAL.Repository.Role;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
	[Authorize]
	public class PrescriptionsController : Controller
	{
		private readonly string ConnectionString;
		public IConfiguration Configuration;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private PriceGroupCompanyRepository priceGroupCompanyRepository;
		private IServicePriceGroupRepository _servicePriceGroupRepository;
		PatientCardRepo patientCardRepo;
		PatientDiagnoseRel patientDiagnoseRel;
		ServicesRepo servicesRepo;
		PatientRepo patientRepo;
		private readonly IPatientCardService _patientCardService;
		private readonly IRoleRepository _roleRepository;
		private readonly IPatientCardServiceRelRepository _patientCardServiceRelRepository;
		private readonly IPatientService _patientService;
		public PrescriptionsController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository, IPatientCardService patientCardService, IServicePriceGroupRepository servicePriceGroupRepository, IPatientCardServiceRelRepository patientCardServiceRelRepository, IPatientService patientService)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
			_servicePriceGroupRepository = servicePriceGroupRepository;
			patientCardRepo = new PatientCardRepo(ConnectionString);
			patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
			servicesRepo = new ServicesRepo(ConnectionString);
			patientRepo = new PatientRepo(ConnectionString);
			_patientCardService = patientCardService;
			_roleRepository = roleRepository;
			_patientCardServiceRelRepository = patientCardServiceRelRepository;
			_patientService = patientService;
		}
		// GET: /<controller>/
		public async Task<IActionResult> Index(int patientID, string search = null)
		{
			if (User.Identity.IsAuthenticated)
			{
				var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
				var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
				var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
				var roles = userRoles.Select(r => r.id);
				List<PatientDocDTO> response = new List<PatientDocDTO>();

				if (roles.Contains(7) || roles.Contains(3))
				{
					//response = await _patientCardService.GetAllPatientsCards(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")), patientID);
					response = await _patientCardService.GetAllPatientsCards(organizationID, patientID, 0, search);



					//ViewBag.patientFullname = patientFullName;
				}
				else if (roles.Contains(4))
				{
					response = await _patientCardService.GetAllPatientsCards(organizationID, patientID, userID, search);
					//ViewBag.patientFullname = patientFullName;
				}
				var vm = new PrescriptionsViewModel()
				{
					PatientCards = response,
					PatientId = patientID,
					SearchText = search,
					isDoctor = roles?.Contains(4) ?? false,
				};
				return View(vm);

			}
			else
			{
				return RedirectToAction("Index", "Login");
			}
		}



		[HttpPost]

		public async Task<IActionResult> AddPrescription(int requestTypeID, long patientID, long cardID, int serviceID, string note)
		{
			if (User.Identity.IsAuthenticated)
			{

				try
				{


					var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
					var organizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));

					if (cardID == 0)
					{
						cardID = patientCardRepo.CreatePatientCard(requestTypeID, userID, patientID, organizationID, serviceID, note: note);
					}


					var serviceInserted = await _patientCardServiceRelRepository.InsertServiceToPatientCard(cardID, serviceID, 0, 0, userID, 0);
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

		public async Task<IActionResult> RemoveCard(int cardId)
		{
			if (User.Identity.IsAuthenticated)
			{

				try
				{
					bool result = await _patientCardService.RemoveAsync(cardId);

					return Ok(result);
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
		public async Task<IActionResult> GetPresctiptionsByDate([FromQuery] DateTime date)
		{

			var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
			var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
			var roles = userRoles.Select(r => r.id);

			List<AppointmentViewModel> result = new List<AppointmentViewModel>();
			//List<PatientCardDAO> result = new List<PatientCardDAO>();

			if (roles.Contains(7) || roles.Contains(3))
			{
				result = await _patientCardService.GetCardsByDate(date, organizationID, 0);

			}
			else if (roles.Contains(4))
			{
				result = await _patientCardService.GetCardsByDate(date, organizationID, userID);
			}


			return Ok(result);

		}



		[HttpGet]
		public async Task<IActionResult> GetCardsByRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
		{
			var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
			var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
			var roles = userRoles.Select(r => r.id);

			List<AppointmentViewModel> result = new List<AppointmentViewModel>();

			if (roles.Contains(7) || roles.Contains(3))
			{
				result = await _patientCardService.GetCardsByRange(startDate, endDate, organizationID, 0);

			}
			else if (roles.Contains(4))
			{
				result = await _patientCardService.GetCardsByRange(startDate, endDate, organizationID, userID);
			}


			return Ok(result);
		}

	}
}

