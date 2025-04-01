using Medicloud.BLL.Models;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    [Authorize]
    public class PricingController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private IServicePriceGroupRepository _servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        public PricingController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IServicePriceGroupRepository servicePriceGroupRepository)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            _servicePriceGroupRepository = servicePriceGroupRepository;
            patientCardRepo = new PatientCardRepo(ConnectionString);
        }

        public IActionResult Index()
        {
            return View();
        }

		[HttpGet]
		public IActionResult SuccessPayment()
		{
			return View();
		}

		[HttpGet]
		public IActionResult FailedPayment()
		{
			return View();
		}

		[HttpGet]
		public IActionResult PendingPayment()
		{
			return View();
		}
	}
}

