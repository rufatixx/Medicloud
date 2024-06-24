using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore;
using Medicloud.Areas.Admin.Model;
using Medicloud.BLL.Service;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PersonalController : Controller
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;

        PersonalPageDTO response;
        UserRepo personalRepo ;
        SpecialityRepo specialityRepo ;
        OrganizationService organizationService;
        public PersonalController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;


             response = new PersonalPageDTO();
             personalRepo = new UserRepo(_connectionString);
             specialityRepo = new SpecialityRepo(_connectionString);
             organizationService = new OrganizationService(_connectionString);
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
        [Authorize]
        public IActionResult Index()
        {

        
                
              
                response.personalList = personalRepo.GetUserList(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")));
                response.specialityList = specialityRepo.GetSpecialities();
                response.organizationList = organizationService.GetAllOrganizations();
              
                return View(response);


            



           
        }
        [HttpGet]
        public IActionResult OrganizationsByUser(int personalID)
        {
            if (User.Identity.IsAuthenticated)
            {
                OrganizationRepo organizationRepo = new OrganizationRepo(_connectionString);
                
                return Ok(organizationRepo.GetOrganizationListByUser(personalID));

            }

            return Unauthorized();



        }
        [HttpGet]
        public IActionResult DepartmentsByOrganization(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                DepartmentsRepo departmentsRepo = new DepartmentsRepo(_connectionString);

                return Ok(departmentsRepo.GetDepartmentsByOrganization(organizationID));

            }
           
                return Unauthorized();
       



        }
        [HttpPost]
        public IActionResult Add(string name, string surname, string father,int specialityID, string passportSerialNum, string fin, string phone, string email, string bDate, string username, string pwd, int isUser,int isDr,int isAdmin)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserRepo personal = new UserRepo(_connectionString);


               
                long newUserID = personal.InsertUser(name, surname, father, specialityID, passportSerialNum, fin, phone, email, bDate, username, pwd, isUser, isDr, isAdmin);
                if (newUserID>0)
                {
                    return Ok(organizationService.LinkOrganizationToUser(newUserID, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")))); 
                }
                else
                {
                    return Ok(newUserID);//not inserted
                }

               

            }
           
                return Unauthorized();
          



        }
        [HttpPost]
        public IActionResult AddOrganizationToUser(long userID,int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                OrganizationRepo organization = new OrganizationRepo(_connectionString);

                return Ok(organization.InsertOrganizationToUser(userID, organizationID));

            }
           
                return Unauthorized();
          



        }
        [HttpPost]
        public IActionResult RemoveOrganizationFromUser(int userID, int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                OrganizationRepo organization = new OrganizationRepo(_connectionString);

                return Ok(organization.RemoveOrganizationFromUser(userID, organizationID));

            }
           
                return Unauthorized();
           


        }
        [HttpGet]
        public IActionResult GetDepartmentsByUser(int userID)
        {
            if (User.Identity.IsAuthenticated)
            {

                UserDepRelRepo userDepRel = new UserDepRelRepo(_connectionString);

                return Ok(userDepRel.GetUserDepartments(userID));

            }
          
                return Unauthorized();
           


        }
        [HttpGet]
        public IActionResult GetAllKassaByOrganization(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {

                KassaRepo kassaRepo = new KassaRepo(_connectionString);

                return Ok(kassaRepo.GetAllKassaListByOrganization(organizationID));

            }
           
                return Unauthorized();



        }
        [HttpGet]
        public IActionResult GetUserKassaByOrganization(int organizationID,int userID)
        {
            if (User.Identity.IsAuthenticated)
            {

                KassaRepo kassaRepo = new KassaRepo(_connectionString);

                return Ok(kassaRepo.GetUserKassaByOrganization(organizationID,userID));

            }
           
                return Unauthorized();
           

        }
        [HttpPost]
        public IActionResult AddDepToUser(int userID, int depID, bool read_only, bool full_access)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserDepRelRepo departmentsRepo = new UserDepRelRepo(_connectionString);

                return Ok(departmentsRepo.InsertDepToUser(userID, depID,read_only,full_access));

            }
            
                return Unauthorized();




        }
        [HttpPost]
        public IActionResult AddKassaToUser(int userID, int kassaID, bool read_only, bool full_access)
        {
            if (User.Identity.IsAuthenticated)
            {
                KassaRepo kassaRepo = new KassaRepo(_connectionString);

                return Ok(kassaRepo.InsertKassaToUser(userID, kassaID, read_only, full_access));

            }
           
                return Unauthorized();
          


        }
        [HttpPost]
        public IActionResult RemoveKassaFromUser(int userID, int kassaID)
        {
            if (User.Identity.IsAuthenticated)
            {
                KassaRepo kassaRepo = new KassaRepo(_connectionString);

                return Ok(kassaRepo.RemoveKassaFromUser(userID, kassaID));

            }
           
                return Unauthorized();
            


        }
        [HttpPost]
        public IActionResult RemoveDepFromUser(int userID, int depID)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserDepRelRepo departmentsRepo = new UserDepRelRepo(_connectionString);

                return Ok(departmentsRepo.RemoveDepFromUser(userID, depID));

            }
                return Unauthorized();
          


        }
        
        [HttpPost]
        public IActionResult UpdateUser(int userID, string name, string surname, string father, int specialityID, string passportSerialNum, string fin, string mobile, string email, string bDate, string username, int isUser, int isDr, int isActive,int isAdmin)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserRepo user = new UserRepo(_connectionString);
   
                return Ok(user.UpdateUser(userID, name, surname, father, specialityID, passportSerialNum, fin, mobile, email, bDate, username, isUser, isDr, isActive, isAdmin));

            }
           
                return Unauthorized();
            



        }
        [HttpPost]
        public IActionResult UpdateUserPwd(int userID, string pwd)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserRepo user = new UserRepo(_connectionString);

                return Ok(user.UpdateUserPwd(userID,pwd));

            }
            
                return Unauthorized();
           



        }

    }
}

