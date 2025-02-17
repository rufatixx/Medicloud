
using Medicloud.BLL.DTO;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.Services;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Business.Controllers
{
    [Area("Business")]
    public class ServicesController : Controller
    {
        private readonly string ConnectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        private ServiceGroupsRepo sgRepo;
        private ServiceTypeRepo stRepo;
        private ServicesRepo sRepo;
        private OOrganizationService organizationService;
        private DepartmentsRepo departmentsRepo;
		//Communications communications;

		private readonly IServicesService _servicesService;
		public ServicesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServicesService servicesService)
		{
			Configuration = configuration;
			ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			sgRepo = new ServiceGroupsRepo(ConnectionString);
			sRepo = new ServicesRepo(ConnectionString);
			organizationService = new OOrganizationService(ConnectionString);
			departmentsRepo = new DepartmentsRepo(ConnectionString);
			stRepo = new ServiceTypeRepo(ConnectionString);
			_servicesService = servicesService;
			//communications = new Communications(Configuration, _hostingEnvironment);
		}

		[Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
            int userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
            if (userID >0)
            {
               
                return View(organizationService.GetOrganizationsByUser(userID));
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

		public async Task<IActionResult> RemoveService( int organizationId, int serviceId)
		{

			bool isRemoved = await _servicesService.RemoveServiceFromOrg(organizationId,serviceId);
			return Ok(isRemoved);
		}

		[HttpGet]

		public async Task<IActionResult> GetService(int serviceId)
		{

			var result=await _servicesService.GetServiceById(serviceId);
			return Ok(result);
		}


		[HttpPost]

		public async Task<IActionResult> AddService([FromBody] AddServiceDTO dto)
		{

			int newId = await _servicesService.AddServiceAsync(dto);
			return Ok(newId);
		}




		[HttpPost]

		public async Task<IActionResult> UpdateService([FromBody] AddServiceDTO dto)
		{

			bool updated = await _servicesService.UpdateService(dto);
			return Ok(updated);
		}



		//[HttpPost]
  //      public IActionResult UpdateService([FromBody] ServiceObj service)
  //      {
  //          if (User.Identity.IsAuthenticated)
  //          {
  //              // Update the service in the repository or database
  //              int updateResult = sRepo.UpdateService(service);

  //              if (updateResult > 0)
  //              {
  //                  return Ok(); // Return HTTP 200 OK if the update was successful
  //              }
  //              else if (updateResult == -1)
  //              {
  //                  // A matching record already exists
  //                  // Return 409 Conflict status with an error message
  //                  return StatusCode(409, "Xidmət artıq mövcuddur");

  //              }
  //              else
  //              {
  //                  return BadRequest("Xəta baş verdi"); // Return HTTP 400 Bad Request if the update failed
  //              }
              
  //          }

  //          return Unauthorized();
            
           
  //      }

        public IActionResult GetDepartmentsInOrganization(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                List<Department> departments = departmentsRepo.GetDepartmentsByOrganization(organizationID);
                return Ok(departments);
            }

            return Unauthorized();
            



        }

        public IActionResult GetDepartmentsInService(int serviceID)
        {
            if (User.Identity.IsAuthenticated)
            {
                List<Department> departments = departmentsRepo.GetDepartmentsInService(serviceID);
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
        public IActionResult GetServiceGroups(int organizationID)
        {
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

