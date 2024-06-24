using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore.Models;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Collections;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    public class KassaController : Controller
    {

        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        PatientCardRepo patientRequestRepo;
        PaymentOperationsRepo paymentOperationsRepo;
        KassaRepo kassaRepo;
        public KassaController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            patientRequestRepo = new PatientCardRepo(ConnectionString);
            paymentOperationsRepo = new PaymentOperationsRepo(ConnectionString);
            kassaRepo = new KassaRepo(ConnectionString);
        }
        [Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
            var selectedKassaID = HttpContext.Session.GetString("Medicloud_kassaID");
            if (!string.IsNullOrEmpty(selectedKassaID))
            {
                return View();
            }
            else
            {
                ViewBag.warningText = "Sizin heç bir kassaya icazəniz yoxdur";
                return View();
            }

           
        }



        [HttpPost]
        public ActionResult<List<PatientCardDTO>> GetDebtorPatients()
        {
            if (User.Identity.IsAuthenticated)
            {
                PatientCardRepo patientRequestRepo = new PatientCardRepo(ConnectionString);

                return patientRequestRepo.GetDebtorPatientCards(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")));
            }
            else
            {
                return Unauthorized();
            }

        }
        [HttpPost]
        public ActionResult<List<PaymentTypeDTO>> GetPaymentTypes()
        {
            if (User.Identity.IsAuthenticated)
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


        public List<PatientServiceDTO> CalculatePayments(List<PatientServiceDTO> services, double paymentAmount)
        {
            double totalDebt = services.Sum(service => double.Parse(service.debt, CultureInfo.InvariantCulture));
            double paymentLeft = paymentAmount;

            foreach (var service in services)
            {
                double debt = double.Parse(service.debt, CultureInfo.InvariantCulture);
                if (debt == 0)
                {
                    // Если долг по услуге уже погашен, пропускаем ее
                    service.CurrentPaymentAmount = "0";
                    continue;
                }

                if (paymentAmount >= totalDebt)
                {
                    // Если платеж покрывает весь долг
                    service.CurrentPaymentAmount = service.debt; // Весь долг по услуге покрывается
                    service.totalPaid = (double.Parse(service.totalPaid, CultureInfo.InvariantCulture) + double.Parse(service.debt, CultureInfo.InvariantCulture)).ToString(CultureInfo.InvariantCulture);
                    service.debt = "0";
                    service.isPaid = true;
                }
                else if (paymentLeft > 0)
                {
                    // Частичное покрытие долга
                    double paymentForService = Math.Min(debt, paymentLeft);
                    service.CurrentPaymentAmount = paymentForService.ToString(CultureInfo.InvariantCulture);
                    service.totalPaid = (double.Parse(service.totalPaid, CultureInfo.InvariantCulture) + paymentForService).ToString(CultureInfo.InvariantCulture);
                    service.debt = (debt - paymentForService).ToString(CultureInfo.InvariantCulture);
                    service.isPaid = double.Parse(service.debt, CultureInfo.InvariantCulture) == 0;
                    paymentLeft -= paymentForService;
                }
                else
                {
                    // Если оставшийся платеж не покрывает долг
                    service.CurrentPaymentAmount = "0";
                }
            }

            return services;
        }



        [HttpPost]
        public ActionResult<bool> AddIncome(long patientCardId, long kassaID, long payment_typeID, long patientID, List<PatientServiceDTO> services, string paymentAmount)
        {
            bool response = false;
            if (User.Identity.IsAuthenticated)
            {

                if (Convert.ToDouble(paymentAmount) > 0 && services.Count > 0)
                {
                    var processedServices = CalculatePayments(services,Convert.ToDouble(paymentAmount));


                    response = paymentOperationsRepo.InsertPaymentOperation(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_kassaID")), payment_typeID, patientID, processedServices, paymentAmount, patientCardId);

                    if (response)
                    {
                        return Ok(response);




                    }
                }
                return Ok(response);
            }
            else
            {

                return Unauthorized();

            }

          
        }

    }
}

