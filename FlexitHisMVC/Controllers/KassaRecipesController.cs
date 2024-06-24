using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    public class KassaRecipesController : Controller
    {
        
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        PaymentOperationsRepo paymentOperationsRepo;
        public KassaRecipesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
           paymentOperationsRepo = new PaymentOperationsRepo(ConnectionString);
        }

        [Authorize]
        // GET: /<controller>/
        public IActionResult Index()
        {
            var selectedKassaID = HttpContext.Session.GetString("Medicloud_kassaID");
            if (!string.IsNullOrEmpty(selectedKassaID))
            {
                KassaDTO response = paymentOperationsRepo.GetPaymentOperations(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")), Convert.ToInt64(HttpContext.Session.GetString("Medicloud_kassaID")));

                return View(response);
            }
            else
            {
                ViewBag.warningText = "Sizin heç bir kassaya icazəniz yoxdur";
                return View();
            }

          
        }


      

    }
}

