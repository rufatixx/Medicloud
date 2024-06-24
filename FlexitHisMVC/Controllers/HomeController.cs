using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Medicloud.Models;
using Microsoft.AspNetCore.Authorization;
using Medicloud.Models.Repository;
using System.Configuration;
using Medicloud.Models.ViewModels;
using Medicloud.Data;

namespace Medicloud.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        string _connectionString;
        PatientRepo patientRepo;
        PatientCardRepo patientCardRepo;
        PaymentOperationsRepo paymentOperationsRepo;
        ServicesRepo servicesRepo;
       
        public HomeController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _connectionString = configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            
            patientRepo = new PatientRepo(_connectionString);
            patientCardRepo = new PatientCardRepo(_connectionString);
            paymentOperationsRepo = new PaymentOperationsRepo(_connectionString);
            servicesRepo = new ServicesRepo(_connectionString);
        }

        public IActionResult Index()
        {

            var viewModel = new HomePageViewModel
            {
                patientStatisticsDTO = patientRepo.GetPatientStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))),
                patientCardStatisticsDTO = patientCardRepo.GetPatientCardStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))),
                paymentOperationStatisticsDTO = paymentOperationsRepo.GetPaymentOperationsStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))),
                top5sellingServiceStatistics = servicesRepo.GetTop5SellingServiceStatistics(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))),
                dailyIncomeStatistics = paymentOperationsRepo.GetDailyStatistics(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_kassaID"))),
                weeklyIncomeStatistics = paymentOperationsRepo.GetWeeklyStatistics(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_kassaID")))
                
                // Populate other lists and properties as needed
            };

            return View(viewModel);
           
          
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

}