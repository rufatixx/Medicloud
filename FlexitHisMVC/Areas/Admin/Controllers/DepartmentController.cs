using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore.Models;
using FlexitHisCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models;
using FlexitHisMVC.Areas.Admin.Model;
using FlexitHisMVC.Data;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DepartmentController : Controller
    {
        private readonly string ConnectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        public DepartmentController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                return View();

            }
            else
            {
                return RedirectToAction("Index", "Login", new { area = "" });
            }
        }

        [HttpPost]
        [Route("admin/departments/getBuildings")]
        public IActionResult GetBuildings(long hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                BuildingRepo select = new BuildingRepo(ConnectionString);
                var list = select.GetBuildings(hospitalID);
                list.Reverse();
                ResponseDTO<Building> response = new ResponseDTO<Building>();
                response.data = new List<Building>();
                response.data.AddRange(list);
                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost]
        [Route("admin/departments/getDepartments")]
        public IActionResult GetDepartments(int buildingID, int depTypeID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {

                DepartmentsRepo select = new DepartmentsRepo(ConnectionString);
                var list = select.GetDepartments();

                list.Reverse();
                ResponseDTO<Department> response = new ResponseDTO<Department>();
                response.data = new List<Department>();
                response.data.AddRange(list);
                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost]
        [Route("admin/departments/getDepartmentTypes")]
        public IActionResult GetDepartmentTypes(int buildingID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                DepartmentTypeRepo select = new DepartmentTypeRepo(ConnectionString);
                var list = select.GetDepartmentTypes();

                list.Reverse();
                ResponseDTO<DepartmentType> response = new ResponseDTO<DepartmentType>();
                response.data = new List<DepartmentType>();
                response.data.AddRange(list);
                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }


        }

        [HttpPost]
        [Route("admin/departments/getDepartmentsInfoByBuilding")]
        public ActionResult<ResponseDTO<Department>> GetDepartmentInfo(int buildingID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                DepartmentsRepo select = new DepartmentsRepo(ConnectionString);
                var list = select.GetDepartmentsByBuilding(buildingID);

                ResponseDTO<Department> response = new ResponseDTO<Department>();
                response.data = new List<Department>();
                response.data.AddRange(list);
                return Ok(response);
                

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [Route("admin/departments/updateDepartment")]
        public IActionResult UpdateDepartment(int id, int gender, long buildingID, int depTypeID, int drIsRequired, int isActive, int isRandevuActive)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                DepartmentsRepo update = new DepartmentsRepo(ConnectionString);
                var response = update.UpdateDepartments(id, gender, buildingID, depTypeID, drIsRequired, isActive, isRandevuActive);

                
                    return Ok(response);
                
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost]
        [Route("admin/departments/insertDepartment")]
        public IActionResult InsertDepartment(long buildingID, string depName, int depTypeID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                DepartmentsRepo insert = new DepartmentsRepo(ConnectionString);
                var response = insert.InsertDepartments(buildingID, depName, depTypeID);

               
                    return Ok(response);
                
            }
            else
            {
                return Unauthorized();
            }


        }
    }
}

