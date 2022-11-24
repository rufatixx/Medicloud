using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore.Models;
using FlexitHisCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using FlexitHisMVC.Models.NewPatient;
using FlexitHisMVC.Models;
using FlexitHisMVC.Data;
using FlexitHisMVC.Models;
using FlexitHisMVC.Repository;
using FlexitHisMVC.Models.DTO;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Controllers
{

    public class NewPatientController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public NewPatientController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("newPatient/getPageModel")]
        public ActionResult<NewPatientViewDTO> getPageModel()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                NewPatientViewDTO pageStruct = new NewPatientViewDTO();
                pageStruct.requestTypes = new List<RequestType>();
                pageStruct.personal = new List<Personal>();
                pageStruct.departments = new List<UserDepRel>();
                pageStruct.referers = new List<Personal>();
                pageStruct.services = new List<Service>();

                RequestTypeRepo requestTypeDAO = new RequestTypeRepo(ConnectionString);

                pageStruct.requestTypes.AddRange(requestTypeDAO.GetRequestType());

                ServicesRepo servicesDAO = new ServicesRepo(ConnectionString);

                pageStruct.services.AddRange(servicesDAO.GetServices());


                UserDepRelRepo departmentsDAO = new UserDepRelRepo(ConnectionString);

                pageStruct.departments = departmentsDAO.GetUserDepartments(1);

                UserRepo personalDAO = new UserRepo(ConnectionString);

                pageStruct.personal.AddRange(personalDAO.GetPersonalList());


                pageStruct.referers.AddRange(personalDAO.GetRefererList());



                pageStruct.status = 1;

                return pageStruct;
            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpPost]
        [Route("newPatient/add")]
        public IActionResult AddPatient([FromBody] AddPatientDTO newPatient)
        {
            ResponseDTO<int> response = new ResponseDTO<int>();
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PatientRepo insert = new PatientRepo(ConnectionString);

                long lastID = insert.InsertPatient(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), newPatient);


                if (lastID > 0)
                {
                    PatientRequestRepo patientRequestDAO = new PatientRequestRepo(ConnectionString);
                    bool inserted = patientRequestDAO.InsertPatientRequest(newPatient, Convert.ToInt32(HttpContext.Session.GetInt32("userid")), lastID);


                    response.status = 1; //inserted


                }
                else
                {
                    response.status = 2; // not inserted (Duplicate)
                }


                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        [Route("newPatient/search")]
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


    }
}

