using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Organization;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Medicloud.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CompanyController : Controller
    {

        private readonly string _connectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        BuildingRepo buildingRepo;
        IOrganizationService _organizationService;
        //Communications communications;
        public CompanyController(IConfiguration configuration, IOrganizationService organizationService, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            buildingRepo = new BuildingRepo(_connectionString);
            _organizationService = organizationService;
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
                obj.organizations = _organizationService.GetAllOrganizations();
                obj.buildings = buildingRepo.GetAllBuildings();
                return Ok(obj);

            }

            return Unauthorized();



        }

        [HttpPost]
        [Route("admin/companies/getCompanyGroups")]
        public IActionResult getCompanyGroups(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                CompanyGroupRepo select = new CompanyGroupRepo(_connectionString);
                List<CompanyGroupDAO> list = select.GetCompanyGroups(organizationID);
                ResponseDTO<CompanyGroupDAO> response = new ResponseDTO<CompanyGroupDAO>();
                response.data = new List<CompanyGroupDAO>();
                response.data = list;

                return Ok(response);


            }

            return Unauthorized();


        }
        [HttpPost]
        [Route("admin/companies/insertCompanyGroup")]
        public IActionResult insertCompanyGroup(int organizationID, string cGroupName, int cGroupType)
        {
            if (User.Identity.IsAuthenticated)
            {
                CompanyGroupRepo insert = new CompanyGroupRepo(_connectionString);
                var response = insert.InsertCompanyGroup(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), organizationID, cGroupName, cGroupType);


                return Ok(response);


            }

            return Unauthorized();


        }
        [HttpPost]
        [Route("admin/companies/insertCompany")]
        public IActionResult insertCompany(int organizationID, string companyName, int cGroupID)
        {
            if (User.Identity.IsAuthenticated)
            {
                CompanyRepo insert = new CompanyRepo(_connectionString);
                var response = insert.InsertCompany(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), organizationID, companyName, cGroupID);


                return Ok(response);

            }

            return Unauthorized();


        }
        [HttpPost]
        [Route("admin/companies/getCompanies")]
        public IActionResult getCompanies(int organizationID)
        {
            if (User.Identity.IsAuthenticated)
            {
                CompanyRepo select = new CompanyRepo(_connectionString);
                var list = select.GetCompanies(organizationID);
                ResponseDTO<CompanyDAO> response = new ResponseDTO<CompanyDAO>();
                response.data = new List<CompanyDAO>();
                response.data = list;
                return Ok(response);

            }

            return Unauthorized();


        }
        [HttpPost]
        [Route("admin/companies/updateCompanyGroup")]
        public IActionResult UpdateCompanyGroup(int organizationID, int id, string name, int isActive)
        {
            if (User.Identity.IsAuthenticated)
            {
                CompanyGroupRepo insert = new CompanyGroupRepo(_connectionString);
                var response = insert.UpdateCompanyGroup(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), organizationID, id, name, isActive);


                return Ok(response);

            }

            return Unauthorized();


        }
        [HttpPost]
        [Route("admin/companies/updateCompany")]
        public IActionResult UpdateCompany(int organizationID, int id, string name, int isActive)
        {
            if (User.Identity.IsAuthenticated)
            {
                CompanyRepo insert = new CompanyRepo(_connectionString);
                var response = insert.UpdateCompany(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), organizationID, id, name, isActive);
                return Ok(response);

            }

            return Unauthorized();


        }


    }
}

