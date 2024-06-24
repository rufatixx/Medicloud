using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore.Models;
using FlexitHisCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using Medicloud.Models;
using Medicloud.Areas.Admin.Model;
using Medicloud.Data;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Medicloud.DAL.Repository;
using Medicloud.BLL.Service;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DepartmentController : Controller
    {
        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        BuildingRepo buildingRepo ;
        OrganizationService organizationService;
        DepartmentsRepo departmentsRepo;
        DepartmentTypeRepo departmentTypeRepo;

        //Communications communications;
        public DepartmentController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
             buildingRepo = new BuildingRepo(_connectionString);
             organizationService = new OrganizationService(_connectionString);
            departmentsRepo = new DepartmentsRepo(_connectionString);
            departmentTypeRepo = new DepartmentTypeRepo(_connectionString);
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
        [Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
                return View();
        }

        [HttpGet]
        
        public IActionResult GetAllOrganizationsWithBuildings()
        {
            if (User.Identity.IsAuthenticated)
            {
                
                dynamic obj = new System.Dynamic.ExpandoObject();
                obj.organizations = organizationService.GetAllOrganizations();
                obj.buildings = buildingRepo.GetAllBuildings();
               return Ok(obj);

            }
           
                return Unauthorized();
       

        }

        [HttpPost]
        [Route("admin/departments/getBuildings")]
        public IActionResult GetBuildings(long organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
               
                var list = buildingRepo.GetBuildings(organizationID);
                list.Reverse();
                ResponseDTO<Building> response = new ResponseDTO<Building>();
                response.data = new List<Building>();
                response.data.AddRange(list);
                return Ok(response);

            }
          
                return Unauthorized();
           


        }
       
        [HttpPost]
        [Route("admin/departments/getDepartmentTypes")]
        public IActionResult GetDepartmentTypes(int buildingID)
        {
            if (User.Identity.IsAuthenticated)
            {
              
                var list = departmentTypeRepo.GetDepartmentTypes();

                list.Reverse();
                ResponseDTO<DepartmentType> response = new ResponseDTO<DepartmentType>();
                response.data = new List<DepartmentType>();
                response.data.AddRange(list);
                return Ok(response);

            }
           
                return Unauthorized();
           

        }

        [HttpPost]
        [Route("admin/departments/getDepartmentsInfoByBuilding")]
        public ActionResult<ResponseDTO<Department>> GetDepartmentsInfoByBuilding(int buildingID)
        {
            if (User.Identity.IsAuthenticated)
            {
                
                var list = departmentsRepo.GetDepartmentsByBuilding(buildingID);

                ResponseDTO<Department> response = new ResponseDTO<Department>();
                response.data = new List<Department>();
                response.data.AddRange(list);
                return Ok(response);
                

            }
           
                return Unauthorized();
          
        }


        [HttpPost]
        [Route("admin/departments/updateDepartment")]
        public IActionResult UpdateDepartment(int id, int gender, long buildingID, int depTypeID, int drIsRequired, int isActive, int isRandevuActive)
        {
            if (User.Identity.IsAuthenticated)
            {
            
                var response = departmentsRepo.UpdateDepartments(id, gender, buildingID, depTypeID, drIsRequired, isActive, isRandevuActive);

                
                    return Ok(response);
                
            }
            
                return Unauthorized();
           

        }

        [HttpPost]
        [Route("admin/departments/insertDepartment")]
        public IActionResult InsertDepartment(long buildingID, string name, int depTypeID)
        {
            if (User.Identity.IsAuthenticated)
            {
               
                var response = departmentsRepo.InsertDepartments(buildingID, name, depTypeID);

               
                    return Ok(response);
                
            }
          
                return Unauthorized();
          


        }
    }
}

