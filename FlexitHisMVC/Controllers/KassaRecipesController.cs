using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Controllers
{
    public class KassaRecipesController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public KassaRecipesController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;

        }
        [HttpPost]
        public ActionResult<KassaDTO> GetPaymentOperations(long kassaID)
        {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                PaymentOperationsRepo paymentOperationsRepo = new PaymentOperationsRepo(ConnectionString);
                KassaDTO response = paymentOperationsRepo.GetPaymentOperations(Convert.ToInt32(HttpContext.Session.GetInt32("userid")) ,kassaID);

          
                    return Ok(response);
                
            }
            else
            {
                return Unauthorized();
            }


        }
    }
}

