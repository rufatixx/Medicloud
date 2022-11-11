using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore.Models;
using FlexitHisCore.Repositories;
using Microsoft.AspNetCore.Mvc;
using FlexitHisMVC.Models.General;
using FlexitHisMVC.Models;
using FlexitHisMVC.Data;

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
                return RedirectToAction("Index", "Login", new { Area = "" });
            }
        }
        private readonly string ConnectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        public CompanyController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }


        [HttpPost]
        [Route("admin/companies/getCompanyGroups")]
        public IActionResult getCompanyGroups(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                CompanyGroupRepo select = new CompanyGroupRepo(ConnectionString);
                List<CompanyGroup> list = select.GetCompanyGroups(hospitalID);
                ResponseDTO<CompanyGroup> response = new ResponseDTO<CompanyGroup>();
                response.data = new List<CompanyGroup>();
                response.data = list;

                return Ok(response);


            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        [Route("admin/companies/insertCompanyGroup")]
        public IActionResult insertCompanyGroup(int hospitalID, string cGroupName, int cGroupType)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                CompanyGroupRepo insert = new CompanyGroupRepo(ConnectionString);
                var response = insert.InsertCompanyGroup(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, cGroupName, cGroupType);


                return Ok(response);


            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        [Route("admin/companies/insertCompany")]
        public IActionResult insertCompany(int hospitalID, string companyName, int cGroupID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                CompanyRepo insert = new CompanyRepo(ConnectionString);
                var response = insert.InsertCompany(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, companyName, cGroupID);


                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        [Route("admin/companies/getCompanies")]
        public IActionResult getCompanies(int hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                CompanyRepo select = new CompanyRepo(ConnectionString);
                var list = select.GetCompanies(hospitalID);
                ResponseDTO<Company> response = new ResponseDTO<Company>();
                response.data = new List<Company>();
                response.data = list;
                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        [Route("admin/companies/updateCompanyGroup")]
        public IActionResult UpdateCompanyGroup(int hospitalID, int id, string name, int isActive)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                CompanyGroupRepo insert = new CompanyGroupRepo(ConnectionString);
                var response = insert.UpdateCompanyGroup(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, id, name, isActive);
               

                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        [Route("admin/companies/updateCompany")]
        public IActionResult UpdateCompany(int hospitalID, int id, string name, int isActive)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                CompanyRepo insert = new CompanyRepo(ConnectionString);
                var response = insert.UpdateCompany(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), hospitalID, id, name, isActive);
                return Ok(response);

            }
            else
            {
                return Unauthorized();
            }

        }


    }
}

