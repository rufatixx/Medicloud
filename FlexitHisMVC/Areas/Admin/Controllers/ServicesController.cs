using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisMVC.Data;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.Domain;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicesController : Controller
    {
        private readonly string ConnectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        private ServiceGroupsRepo sgRepo;
        private ServiceTypeRepo stRepo;
        private ServicesRepo sRepo;
        private HospitalRepo hospitalRepo;
        private DepartmentsRepo departmentsRepo;
        //Communications communications;
        public ServicesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            sgRepo = new ServiceGroupsRepo(ConnectionString);
            sRepo = new ServicesRepo(ConnectionString);
            hospitalRepo = new HospitalRepo(ConnectionString);
            departmentsRepo = new DepartmentsRepo(ConnectionString);
            stRepo = new ServiceTypeRepo(ConnectionString);
            //communications = new Communications(Configuration, _hostingEnvironment);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetInt32("userid"));
            if (userID >0)
            {
               
                return View(hospitalRepo.GetHospitalListByUser(userID));
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }
           
        }

        public IActionResult GetServicesWithServiceGroupName(int hospitalID,int serviceGroupID = 0)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                List<ServiceObj> serviceObjs = sRepo.GetServicesWithServiceGroupName(hospitalID,serviceGroupID);
                return Ok(serviceObjs);
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }
           
        }

        [HttpPost]
        public IActionResult UpdateService([FromBody] ServiceObj service)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                // Update the service in the repository or database
                int updateResult = sRepo.UpdateService(service);

                if (updateResult > 0)
                {
                    return Ok(); // Return HTTP 200 OK if the update was successful
                }
                else if (updateResult == -1)
                {
                    // A matching record already exists
                    // Return 409 Conflict status with an error message
                    return StatusCode(409, "Xidmət artıq mövcuddur");

                }
                else
                {
                    return BadRequest("Xəta baş verdi"); // Return HTTP 400 Bad Request if the update failed
                }
              
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }
           
        }

        public IActionResult GetDepartmentsInHospital(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                List<Department> departments = departmentsRepo.GetDepartmentsByHospital(hospitalID);
                return Ok(departments);
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }



        }

        public IActionResult GetDepartmentsInService(int serviceID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                List<Department> departments = departmentsRepo.GetDepartmentsInService(serviceID);
                return Ok(departments);
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }



        }
        public IActionResult InsertDepartmentToService(int serviceId, int departmentId)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                var updateResult = departmentsRepo.InsertDepartmentToService(serviceId, departmentId);

                if (updateResult)
                {
                    return Ok(); // Return HTTP 200 OK if the update was successful
                }
                else
                {
                    return BadRequest("Service not added"); // Return HTTP 400 Bad Request if the update failed
                }
              
              
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }



        }
        public IActionResult GetServiceGroups(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                List<ServiceGroup> serviceGroups = sgRepo.GetGroupsByHospital(hospitalID);
                return Ok(serviceGroups);
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }
           

            
        }

        public IActionResult GetServiceTypes()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                List<ServiceType> serviceTypes = stRepo.GetServiceTypes();
                return Ok(serviceTypes);
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }



        }

        [HttpPost]
        public IActionResult InsertService([FromBody] ServiceObj serviceObj)
        {

            if (HttpContext.Session.GetInt32("userid") != null)
            {
                var result = sRepo.InsertService(serviceObj);
                if (result>0)
                {
                    return Ok();
                }
                else if (result == -1)
                {
                    // A matching record already exists
                    // Return 409 Conflict status with an error message
                    return StatusCode(409, "Xidmət artıq mövcuddur");

                }
                else
                {
                    return BadRequest("Xəta baş verdi"); // Return HTTP 400 Bad Request if the update failed
                }



            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public IActionResult InserServiceGroup([FromBody]ServiceGroup serviceGroup)
        {
            
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                sgRepo.InsertServiceGroup(serviceGroup);

                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
        public IActionResult DeleteServiceGroup(int sgID)
        {

            if (HttpContext.Session.GetInt32("userid") != null)
            {
                sgRepo.DeleteServiceGroup(sgID);

                return Ok();
            }
            else
            {
                return RedirectToAction("Index", "Login", new { Area = "" });
            }
        }

        //[HttpPost]
        //public IActionResult InsertServiceGroup(long buildingID, string name, int depTypeID)
        //{
        //    if (HttpContext.Session.GetInt32("userid") != null)
        //    {
        //        DepartmentsRepo insert = new DepartmentsRepo(ConnectionString);
        //        var response = insert.InsertDepartments(buildingID, name, depTypeID);


        //        return Ok(response);

        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }


        //}
    }
}

