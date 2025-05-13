using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medicloud.BLL.Models;
using Medicloud.BLL.Services.Abstract;
using Medicloud.BLL.Services.Anamnesis;
using Medicloud.BLL.Services.Patient;
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
	public class PoliclinicController : Controller
	{
		private readonly string ConnectionString;
		public IConfiguration Configuration;
		private readonly IWebHostEnvironment _hostingEnvironment;
		private readonly IPatientCardService _petientCardService;
		private readonly IPatientService _patientService;
		private readonly IAnamnesisService _anamnesisService;
		public PoliclinicController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IPatientCardService petientCardService, IPatientService petientService, IAnamnesisService anamnesisService)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			_petientCardService = petientCardService;
			_patientService = petientService;
			_anamnesisService = anamnesisService;
		}
		[Authorize]
		// GET: /<controller>/
		public async Task<IActionResult> Index(int id = 0)
		{
			int userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
			int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			//PatientCardRepo patientRequestDAO = new PatientCardRepo(ConnectionString);
			//var response = patientRequestDAO.GetPatientsByDr(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));
			var response = await _patientService.GetPatientsWithCardsByDr(organizationId,userId);
			//Console.WriteLine(id);
			var anamnesisFields = await _anamnesisService.GetFieldsWithTemplatesByDoctorId(userId);

			//if(anamnesisFields != null)
			//{
			//	foreach (var item in anamnesisFields)
			//	{
			//		Console.WriteLine(item.name);
			//		Console.WriteLine(item.id);
			//		Console.WriteLine(item.Templates?.Count);
			//	}
			//}
			var vm = new PoliclinicViewModel
			{
				Patients = response,
				SelectedCardId = id,
				AnamnesisFields = anamnesisFields
			};

			if (id > 0)
			{
				var anamnesis=await _anamnesisService.GetAnamnesisByCardId(id);
				vm.CardAnamnesis = anamnesis;
				//if (anamnesis != null)
				//{
				//	foreach (var item in anamnesis)
				//	{
				//		Console.WriteLine(item.id);
				//		Console.WriteLine(item.patientCardId);
				//		Console.WriteLine(item.AnamnesisAnswers.Count);
				//	}
				//	Console.WriteLine();
				//}
			}
			return View(vm);

			//return View();
		}
		[HttpGet]
		public IActionResult SearchDiagnose(string icdID, string name)
		{
			if (User.Identity.IsAuthenticated)
			{
				DiagnoseRepo diagnoseRepo = new DiagnoseRepo(ConnectionString);
				var response = diagnoseRepo.SearchDiagnose(icdID, name);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
			//return View();
		}
		[HttpGet]
		public IActionResult AddDiagnose(int patientID, long diagnoseID)
		{
			if (User.Identity.IsAuthenticated)
			{
				PatientDiagnoseRel patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
				var response = patientDiagnoseRel.InsertPatientToDiagnose(patientID, diagnoseID);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
			//return View();
		}


		[HttpGet]
		public async Task<IActionResult> GetAnamnesisById(int id)
		{
			if (User.Identity.IsAuthenticated)
			{
				var anamnesis = await _anamnesisService.GetAnamnesisById(id);
				return Ok(anamnesis);
			}
			else
			{
				return Unauthorized();
			}
			//return View();
		}

		[HttpPost]
		public async Task<IActionResult> AddAnamnesis([FromBody] AddAnamnesisDTO dto)
		{
			if (dto == null) Console.WriteLine("DTO NULL");
			if (User.Identity.IsAuthenticated)
			{
				int userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
				dto.DoctorId = userId;
				int response = await _anamnesisService.AddAnamnesis(dto);
				//ar response = patientDiagnoseRel.InsertPatientToDiagnose(patientID, diagnoseID);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAnamnesis([FromBody] AddAnamnesisDTO dto)
		{
			if (dto == null) Console.WriteLine("DTO NULL");
			if (User.Identity.IsAuthenticated)
			{
				int userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
				dto.DoctorId = userId;
				int response = await _anamnesisService.UpdateAnamnesis(dto);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
		}

		[HttpPost]
		public async Task<IActionResult> DeleteAnamnesis(int anamnesisId)
		{
			if (User.Identity.IsAuthenticated)
			{
				bool deleted= await _anamnesisService.RemoveAnamnesis(anamnesisId);
				return Ok(deleted);
			}
			else
			{
				return Unauthorized();
			}
		}

		[HttpGet]
		public IActionResult GetDiagnoses(int patientID)
		{
			if (User.Identity.IsAuthenticated)
			{
				PatientDiagnoseRel patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
				var response = patientDiagnoseRel.GetPatientToDiagnose(patientID);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
			//return View();
		}
		[HttpGet]
		public IActionResult DeleteDiagnose(int patientDiagnoseRelID)
		{
			if (User.Identity.IsAuthenticated)
			{
				PatientDiagnoseRel patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
				var response = patientDiagnoseRel.RemovePatientToDiagnose(patientDiagnoseRelID);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
			//return View();
		}
		[HttpGet]
		public IActionResult GetRecords(int patientID)
		{
			if (User.Identity.IsAuthenticated)
			{
				PatientRecordRelRepo patientRecordRelRepo = new PatientRecordRelRepo(ConnectionString);
				var response = patientRecordRelRepo.GetRecords(patientID);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
			//return View();
		}
		[HttpGet]
		public IActionResult DeleteRec(int patientRecRelID)
		{
			if (User.Identity.IsAuthenticated)
			{
				PatientRecordRelRepo patientRecordRelRepo = new PatientRecordRelRepo(ConnectionString);
				var response = patientRecordRelRepo.RemovePatientToRec(patientRecRelID);
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}
			//return View();
		}
		[HttpPost]
		public IActionResult UploadVideo(int patientID, [FromForm] IFormFile videoFile)
		{
			if (User.Identity.IsAuthenticated)
			{
				if (videoFile == null || videoFile.Length == 0)
				{
					return BadRequest("No video file found in the request.");
				}

				var folderName = "patients";
				var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
				var folderPath = Path.Combine(webRootPath, folderName, patientID.ToString());
				if (!Directory.Exists(folderPath))
				{
					Directory.CreateDirectory(folderPath);
				}

				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
				var filePath = Path.Combine(folderPath, fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					videoFile.CopyTo(stream);
				}
				RecordingsRepo recordingsRepo = new RecordingsRepo(ConnectionString);
				var recordingID = recordingsRepo.InsertIntoRecordings(fileName, Path.Combine(folderName, patientID.ToString(), fileName));
				if (recordingID > 0)
				{
					PatientRecordRelRepo patientRecordRelRepo = new PatientRecordRelRepo(ConnectionString);
					var patientRecRel = patientRecordRelRepo.InsertIntoPatientRecordRel(patientID, recordingID);
					if (patientRecRel > 0)
					{
						return Ok($"Video file uploaded successfully: {fileName}");
					}
				}

				return BadRequest($"Video file not uploaded successfully: {fileName}");
			}
			else
			{
				return Unauthorized();


			}
		}

	}
}

