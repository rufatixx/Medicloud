using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Medicloud.Models;
using Microsoft.AspNetCore.Authorization;
using Medicloud.Models.Repository;
using System.Configuration;
using Medicloud.Models.ViewModels;
using Medicloud.Data;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository.Role;

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
        IRoleRepository _roleRepository;
        public HomeController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository)
        {
            _connectionString = configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            
            patientRepo = new PatientRepo(_connectionString);
            patientCardRepo = new PatientCardRepo(_connectionString);
            paymentOperationsRepo = new PaymentOperationsRepo(_connectionString);
            servicesRepo = new ServicesRepo(_connectionString); // NEED TO BE UPDATED TO THE NEW VERSION WITH DI
            _roleRepository = roleRepository;
        }

        public async Task<IActionResult> Index()
        {
            var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
            var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
            var roles = userRoles.Select(r => r.id);

            if (roles.Any())
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
            else
            {
                return RedirectToAction("NoRoleView");
            }
           
          
        }
        public async Task<IActionResult> NoRoleView()
        {
            var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
            var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
            var roles = userRoles.Select(r => r.id);

            if (roles.Any())
            {
                return RedirectToAction("Index");
            }

                return View();
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