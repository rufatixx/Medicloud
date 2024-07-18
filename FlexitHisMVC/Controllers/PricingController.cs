using Medicloud.BLL.Models;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class PricingController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private ServicePriceGroupRepository servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        public PricingController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            servicePriceGroupRepository = new ServicePriceGroupRepository(ConnectionString);
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
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

