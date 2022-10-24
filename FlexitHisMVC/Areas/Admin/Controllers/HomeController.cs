using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHis_API.Models.Db;
using FlexitHis_API.Models.Structs;
using FlexitHis_API.Models.Structs.Admin;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        public HomeController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

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
        public ActionResult<ResponseStruct<Building>> GetDepartments(long hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminSelect select = new AdminSelect(Configuration, _hostingEnvironment);
                ResponseStruct<Building> response = select.GetBuildings(hospitalID);

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost]
        [Route("admin/departments/getDepartments")]
        public ActionResult<ResponseStruct<PatientStruct>> GetBuildings(int buildingID, int depTypeID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminSelect select = new AdminSelect(Configuration, _hostingEnvironment);
                ResponseStruct<Department> response = select.AdminGetDepartments();

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return Unauthorized();
            }


        }
        [HttpPost]
        [Route("admin/departments/getDepartmentTypes")]
        public ActionResult<ResponseStruct<DepartmentType>> GetDepartmentTypes(int buildingID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminSelect select = new AdminSelect(Configuration, _hostingEnvironment);
                ResponseStruct<DepartmentType> response = select.AdminGetDepartmentTypes();

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return Unauthorized();
            }


        }

        [HttpPost]
        [Route("admin/departments/getDepartmentsInfoByBuilding")]
        public ActionResult<ResponseStruct<Department>> GetDepartmentInfo(int buildingID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminSelect select = new AdminSelect(Configuration, _hostingEnvironment);
                ResponseStruct<Department> response = select.GetDepartmentsByBuilding(buildingID);

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }

            }
            else
            {
                return Unauthorized();
            }
        }


        [HttpPost]
        [Route("admin/departments/updateDepartment")]
        public ActionResult<ResponseStruct<int>> UpdateDepartment(int id, int gender, long buildingID, int depTypeID, int drIsRequired, int isActive, int isRandevuActive)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminInsert insert = new AdminInsert(Configuration, _hostingEnvironment);
                ResponseStruct<int> response = insert.UpdateDepartments(id, gender, buildingID, depTypeID, drIsRequired, isActive, isRandevuActive);

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return Unauthorized();
            }

        }

        [HttpPost]
        [Route("admin/departments/insertDepartment")]
        public ActionResult<ResponseStruct<int>> InsertDepartment(long buildingID, string depName, int depTypeID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminInsert insert = new AdminInsert(Configuration, _hostingEnvironment);
                ResponseStruct<int> response = insert.InsertDepartments(buildingID, depName, depTypeID);

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return Unauthorized();
            }


        }

    }
}

