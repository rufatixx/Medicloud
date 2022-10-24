using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using FlexitHis_API.Models.Db;
using FlexitHis_API.Models.Structs;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Controllers
{
 
    public class NewPatientController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public NewPatientController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        [Route("newPatient/getPageModel")]
        public ActionResult<AddNewPatientPageStruct> getPageModel()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                Select select = new Select(Configuration, _hostingEnvironment);
                AddNewPatientPageStruct obj = select.GetNewPatientPage();
                if (obj.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return obj;
                }
            }
            else
            {
                return Unauthorized();
            }
            


        }
        [HttpPost]
        [Route("newPatient/add")]
        public IActionResult AddPatient([FromBody] AddPatientStruct newPatient)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                Insert insert = new Insert(Configuration, _hostingEnvironment);

                ResponseStruct<int> response = insert.InsertOrder(Convert.ToInt32(HttpContext.Session.GetInt32("userid")),newPatient);

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }
            }
            else {
                return Unauthorized();
            }

        }
        [HttpPost]
        [Route("newPatient/search")]
        public ActionResult<ResponseStruct<PatientStruct>> SearchForPatient( string fullNamePattern, long hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {

                Select select = new Select(Configuration, _hostingEnvironment);
                ResponseStruct<PatientStruct> response = select.SearchForPatients( fullNamePattern, hospitalID);

                if (response.status == 3)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(response);
                }
            }
            else {
                return Unauthorized();
            }

        }


    }
}

