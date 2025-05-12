using Medicloud.DAL.Repository.Abstract;
using Medicloud.DAL.Repository.Patient;
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
		private readonly IPatientRepository _patientRepository;

		public ClientsController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServicePriceGroupRepository servicePriceGroupRepository, IPatientCardServiceRelRepository patientCardServiceRelRepository, IPatientRepository patientRepository)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
			_servicePriceGroupRepository = servicePriceGroupRepository;
			patientCardRepo = new PatientCardRepo(ConnectionString);
			patientRepo = new PatientRepo(ConnectionString);
			_patientCardServiceRelRepository = patientCardServiceRelRepository;
			_patientRepository = patientRepository;
		}

		// GET: /<controller>/
		public IActionResult Index()
		{


			var response = patientCardRepo.GetPatientsByOrganization(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));


			return View(response);

		}


		[HttpPost]
		public async Task<IActionResult> AddClient([FromBody] PatientDTO newPatient)
		{

			try
			{
				int userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
				int organzationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

				int newPatientId= await _patientRepository.AddAsync(new()
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
					orgReasonId = newPatient.orgReasonId,
				});
				//var newPatientID = patientRepo.InsertPatient(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), newPatient);

				return Ok(newPatientId);
			}
			catch (Exception ex)
			{
				// Handle the exception and return an appropriate response
				return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
			}


		}




		[HttpPost]
		public async Task<IActionResult> UpdateClient([FromBody] PatientDTO patient)
		{

			try
			{
				//var newPatientID = patientRepo.UpdatePatient(patient);
				int result=await _patientRepository.UpdateAsync(new()
				{
					id=patient.id,
					name = patient.name,
					surname = patient.surname,
					clientEmail = patient.clientEmail,
					bDate = patient.birthDate,
					clientPhone = patient.clientPhone,
					father = patient.father,
					fin = patient.fin,
					genderID = patient.genderID,
					orgReasonId = patient.orgReasonId,
				});
				return Ok(result);
			}
			catch (Exception ex)
			{
				// Handle the exception and return an appropriate response
				return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
			}


		}



	}
}

