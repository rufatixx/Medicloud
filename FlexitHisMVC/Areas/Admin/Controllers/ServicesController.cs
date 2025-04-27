using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Medicloud.Areas.Admin.ViewModels;
using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Organization;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Admin.Controllers
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
        private IOrganizationService _organizationService;
        private DepartmentsRepo departmentsRepo;
        //Communications communications;
        public ServicesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IOrganizationService organizationService)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            sgRepo = new ServiceGroupsRepo(ConnectionString);
            sRepo = new ServicesRepo(ConnectionString);
            _organizationService = organizationService;
            departmentsRepo = new DepartmentsRepo(ConnectionString);
            stRepo = new ServiceTypeRepo(ConnectionString);
            //communications = new Communications(Configuration, _hostingEnvironment);
        }

        [Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
            int organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

            if (userID >0)
            {
                //_organizationService.GetOrganizationsByUser(userID);
                List<ServiceGroup> serviceGroups = sgRepo.GetGroupsByOrganization(organizationID);
                List<DepartmentDAO> departments = departmentsRepo.GetDepartmentsByOrganization(organizationID);
                List<ServiceObj> serviceObjs = sRepo.GetServicesByOrganization(organizationID);

                var vm = new ServicesViewModel
                {
                    ServiceGroups = serviceGroups,
                    Departments=departments,
                    Services=serviceObjs
                };
                return View(vm);
            }
            return NotFound();
         
           
        }

        public IActionResult GetServicesWithServiceGroupName(int organizationID,int serviceGroupID = 0)
        {

             organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
          
            if (User.Identity.IsAuthenticated)
            {
                List<ServiceObj> serviceObjs = sRepo.GetServicesWithServiceGroupName(organizationID,serviceGroupID);
                return Ok(serviceObjs);
            }

            return Unauthorized();
            
           
        }

        [HttpPost]
        public IActionResult UpdateService([FromBody] ServiceObj service)
        {
            if (User.Identity.IsAuthenticated)
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

            return Unauthorized();
            
           
        }

        public IActionResult GetDepartmentsInOrganization(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
                List<DepartmentDAO> departments = departmentsRepo.GetDepartmentsByOrganization(organizationID);
                return Ok(departments);
            }

            return Unauthorized();
            



        }

        public IActionResult GetDepartmentsInService(int serviceID)
        {
            if (User.Identity.IsAuthenticated)
            {

                List<DepartmentDAO> departments = departmentsRepo.GetDepartmentsInService(serviceID);
                return Ok(departments);
            }

            return Unauthorized();
          



        }
        public IActionResult InsertDepartmentToService(int serviceId, int departmentId)
        {
            if (User.Identity.IsAuthenticated)
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
                return Unauthorized();
            }



        }

        public IActionResult DeleteDepartmentFromService(int serviceId, int departmentId)
        {
            if (User.Identity.IsAuthenticated)
            {
                var updateResult = departmentsRepo.DeleteDepartmentFromService(serviceId, departmentId);

                if (updateResult)
                {
                    return Ok(); // Return HTTP 200 OK if the update was successful
                }
                else
                {
                    return BadRequest("Service not deleted"); // Return HTTP 400 Bad Request if the update failed
                }


            }
            else
            {
                return Unauthorized();
            }



        }
        public IActionResult GetServiceGroups(int organizationID=0)
        {
            organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            if (User.Identity.IsAuthenticated)
            {
                List<ServiceGroup> serviceGroups = sgRepo.GetGroupsByOrganization(organizationID);
                return Ok(serviceGroups);
            }
            else
            {
                return Unauthorized();
            }
           

            
        }

        public IActionResult GetServiceTypes()
        {
            if (User.Identity.IsAuthenticated)
            {
                List<ServiceType> serviceTypes = stRepo.GetServiceTypes();
                return Ok(serviceTypes);
            }
            else
            {
                return Unauthorized();
            }



        }

        [HttpPost]
        public IActionResult InsertService([FromBody] ServiceObj serviceObj)
        {

            if (User.Identity.IsAuthenticated)
            {
                serviceObj.organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
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
            if(serviceGroup !=null)
            {
                serviceGroup.organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            }

            if (User.Identity.IsAuthenticated)
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

            if (User.Identity.IsAuthenticated)
            {
                sgRepo.DeleteServiceGroup(sgID);

                return Ok();
            }
            else
            {
                return Unauthorized();
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

