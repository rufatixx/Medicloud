using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisMVC.Data;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.Repository;
using FlexitHisMVC.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Controllers
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
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PatientRequestRepo patientRequestDAO = new PatientRequestRepo(ConnectionString);
                var response = patientRequestDAO.GetPatientsByDr(Convert.ToInt32(HttpContext.Session.GetInt32("userid")));
                return View(response);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
            //return View();
        }
        [HttpGet]
        public IActionResult SearchDiagnose(string icdID,string name)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                DiagnoseRepo diagnoseRepo = new DiagnoseRepo(ConnectionString);
                var response = diagnoseRepo.SearchDiagnose(icdID,name);
                return Ok(response);
            }
            else
            {
                return Unauthorized();
            }
            //return View();
        }
        [HttpGet]
        public IActionResult AddDiagnose(int patientID,long diagnoseID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PatientDiagnoseRel patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
                var response = patientDiagnoseRel.InsertPatientToDiagnose(patientID,diagnoseID);
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
            if (HttpContext.Session.GetInt32("userid") != null)
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
            if (HttpContext.Session.GetInt32("userid") != null)
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
        [HttpPost]
        public IActionResult UploadVideo(int patientID, [FromForm] IFormFile videoFile)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
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

                return Ok($"Video file uploaded successfully: {fileName}");
            }
            else
            {
                return Unauthorized();


            }
        }

    }
}

