using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore;
using FlexitHisMVC.Areas.Admin.Model;
using FlexitHisMVC.Data;
using FlexitHisMVC.Models;
using FlexitHisMVC.Repository;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PersonalController : Controller
    {
        private readonly string ConnectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        public PersonalController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
     
        public IActionResult Index()
        {

            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PersonalPageDTO response = new PersonalPageDTO();
                UserRepo personalRepo = new UserRepo(ConnectionString);
                SpecialityRepo  specialityRepo = new SpecialityRepo(ConnectionString);
                HospitalRepo  hospitalRepo = new HospitalRepo(ConnectionString);
              
                response.personalList = personalRepo.GetPersonalList();
                response.specialityList = specialityRepo.GetSpecialities();
                response.hospitalList = hospitalRepo.GetHospitalList();
              
                return View(response);


            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }


           
        }
        [HttpGet]
        public IActionResult HospitalsByUser(int personalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                HospitalRepo hospitalRepo = new HospitalRepo(ConnectionString);
                
                return Ok(hospitalRepo.GetHospitalListByUser(personalID));

            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }



        }
        [HttpGet]
        public IActionResult DepartmentsByHospital(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                DepartmentsRepo departmentsRepo = new DepartmentsRepo(ConnectionString);

                return Ok(departmentsRepo.GetDepartmentsByHospital(hospitalID));

            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpPost]
        public IActionResult Add(string name, string surname, string father,int specialityID, string passportSerialNum, string fin, string phone, string email, string bDate, string username, string pwd, int isUser,int isDr)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                UserRepo personal = new UserRepo(ConnectionString);
               
                return Ok(personal.InsertPersonal( name,  surname,  father,specialityID,  passportSerialNum,  fin,  phone,  email,  bDate,  username,  pwd,  isUser, isDr));

            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpPost]
        public IActionResult AddHospitalToUser(int userID,int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                HospitalRepo hospital = new HospitalRepo(ConnectionString);

                return Ok(hospital.InsertHospitalToUser(userID, hospitalID));

            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpPost]
        public IActionResult RemoveHospitalFromUser(int userID, int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                HospitalRepo hospital = new HospitalRepo(ConnectionString);

                return Ok(hospital.RemoveHospitalFromUser(userID, hospitalID));

            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpGet]
        public IActionResult GetDepartmentsByUser(int userID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {

                UserDepRelRepo userDepRel = new UserDepRelRepo(ConnectionString);

                return Ok(userDepRel.GetUserDepartments(userID));

            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpGet]
        public IActionResult GetUserKassaByHospital(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {

                KassaRepo kassaRepo = new KassaRepo(ConnectionString);

                return Ok(kassaRepo.GetUserKassaByHospital(hospitalID));

            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost]
        public IActionResult AddDepToUser(int userID, int depID, int read_only, int full_access)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                UserDepRelRepo departmentsRepo = new UserDepRelRepo(ConnectionString);

                return Ok(departmentsRepo.InsertDepToUser(userID, depID,read_only,full_access));

            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpPost]
        public IActionResult RemoveDepFromUser(int userID, int depID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                UserDepRelRepo departmentsRepo = new UserDepRelRepo(ConnectionString);

                return Ok(departmentsRepo.RemoveDepFromUser(userID, depID));

            }
            else
            {
                return Unauthorized();
            }



        }
        
        [HttpPost]
        public IActionResult UpdateUser(int userID, string name, string surname, string father, int specialityID, string passportSerialNum, string fin, string mobile, string email, string bDate, string username, int isUser, int isDr, int isActive)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                UserRepo user = new UserRepo(ConnectionString);
   
                return Ok(user.UpdateUser(userID, name, surname, father, specialityID, passportSerialNum, fin, mobile, email, bDate, username, isUser, isDr, isActive));

            }
            else
            {
                return Unauthorized();
            }



        }
        [HttpPost]
        public IActionResult UpdateUserPwd(int userID, string pwd)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                UserRepo user = new UserRepo(ConnectionString);

                return Ok(user.UpdateUserPwd(userID,pwd));

            }
            else
            {
                return Unauthorized();
            }



        }

    }
}

