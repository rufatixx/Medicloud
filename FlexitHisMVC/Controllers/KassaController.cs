using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore.Models;
using FlexitHisMVC.Data;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Collections;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Controllers
{
    public class KassaController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public KassaController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }

        [HttpPost]
        public ActionResult<List<PatientKassaDTO>> GetDebtorPatients(long hospitalID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PatientRequestRepo patientRequestRepo = new PatientRequestRepo(ConnectionString);

                return patientRequestRepo.GetDebtorPatients(hospitalID);
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        public ActionResult<List<PaymentTypeDTO>> GetPaymentTypes()
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {

                PaymentTypeRepo paymentTypeRepo = new PaymentTypeRepo(ConnectionString);

                List<PaymentTypeDTO> response = paymentTypeRepo.GetPaymentTypes();


                return Ok(response);


            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]

        public ActionResult<bool> AddIncome(long kassaID, long payment_typeID, long patientID)
        {
            bool response = false;
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PaymentOperationsRepo paymentOperationsRepo = new PaymentOperationsRepo(ConnectionString);


                response = paymentOperationsRepo.InsertPaymentOperation(Convert.ToInt32(HttpContext.Session.GetInt32("userid")), kassaID, payment_typeID, patientID);

                if (response)
                {
                    PatientRequestRepo patientRequestRepo = new PatientRequestRepo(ConnectionString);
                    patientRequestRepo.UpdatePatientRequest(patientID);

                }
                else
                {
                    response = false;

                }
            }
            else
            {

                return Unauthorized();

            }

            return Ok(response);
        }

    }
}

