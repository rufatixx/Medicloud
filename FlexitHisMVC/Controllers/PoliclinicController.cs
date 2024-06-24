using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
   
    public class PoliclinicController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public PoliclinicController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }
        [Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
          
                PatientCardRepo patientRequestDAO = new PatientCardRepo(ConnectionString);
                var response = patientRequestDAO.GetPatientsByDr(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")));
                return View(response);
          
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
                var recordingID = recordingsRepo.InsertIntoRecordings(fileName, Path.Combine(folderName, patientID.ToString(),fileName));
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

