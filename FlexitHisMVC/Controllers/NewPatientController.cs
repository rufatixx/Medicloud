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
using FlexitHisMVC.Models.Repository;

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



                pageStruct.status = 1;

                return pageStruct;
            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpPost]
    
        public IActionResult AddPatient([FromBody] AddPatientDTO newPatient)
        {
           
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                ResponseDTO<int> response = new ResponseDTO<int>();
                try
                {
                    PatientRepo insert = new PatientRepo(ConnectionString);
                    long ID;
                    if (newPatient.foundPatientID > 0)
                    {
                        ID = newPatient.foundPatientID;
                       
                    }
                    else
                    {
                        ID = insert.InsertPatient(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), newPatient);
                    }



                    if (ID>0)
                    {
                        PatientRequestRepo patientRequestDAO = new PatientRequestRepo(ConnectionString);
                        bool inserted = patientRequestDAO.InsertPatientRequest(newPatient, Convert.ToInt32(HttpContext.Session.GetInt32("userid")), ID);


                        response.status = 1; //inserted


                    }
                    else
                    {
                        response.status = 2; // not inserted (Duplicate)
                    }


                    

                }
                catch (Exception ex)
                {
                    response.status = 3; // not inserted (Duplicate)
                }
                return Ok(response);

            }
            else
            {
                return Unauthorized();
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


    }
}

