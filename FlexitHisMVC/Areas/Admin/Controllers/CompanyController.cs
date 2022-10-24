using System;
using System.Collections.Generic;
using System.Configuration;
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
    public class CompanyController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Login", new {Area = "" });
            }
        }
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        public CompanyController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }


        [HttpPost]
        [Route("admin/companies/getCompanyGroups")]
        public ActionResult<ResponseStruct<CompanyGroup>> getCompanyGroups(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminSelect select = new AdminSelect(Configuration, _hostingEnvironment);
                ResponseStruct<CompanyGroup> response = select.GetCompanyGroups(hospitalID);

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
        [Route("admin/companies/insertCompanyGroup")]
        public ActionResult<ResponseStruct<int>> insertCompanyGroup(int hospitalID, string cGroupName, int cGroupType)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminInsert insert = new AdminInsert(Configuration, _hostingEnvironment);
                ResponseStruct<int> response = insert.InsertCompanyGroup(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, cGroupName, cGroupType);

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
        [Route("admin/companies/insertCompany")]
        public ActionResult<ResponseStruct<int>> insertCompany(int hospitalID, string companyName, int cGroupID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminInsert insert = new AdminInsert(Configuration, _hostingEnvironment);
                ResponseStruct<int> response = insert.InsertCompany(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, companyName, cGroupID);

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
        [Route("admin/companies/getCompanies")]
        public ActionResult<ResponseStruct<Company>> getCompanies(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminSelect select = new AdminSelect(Configuration, _hostingEnvironment);
                ResponseStruct<Company> response = select.GetCompanies(hospitalID);

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
        [Route("admin/companies/updateCompanyGroup")]
        public ActionResult<ResponseStruct<int>> UpdateCompanyGroup(int hospitalID, int id, string name, int isActive)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminInsert insert = new AdminInsert(Configuration, _hostingEnvironment);
                ResponseStruct<int> response = insert.UpdateCompanyGroup(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, id, name, isActive);

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
        [Route("admin/companies/updateCompany")]
        public ActionResult<ResponseStruct<int>> UpdateCompany(int hospitalID, int id, string name, int isActive)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                AdminInsert insert = new AdminInsert(Configuration, _hostingEnvironment);
                ResponseStruct<int> response = insert.UpdateCompany(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, id, name, isActive);

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

