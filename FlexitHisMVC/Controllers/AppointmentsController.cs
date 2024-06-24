using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

    public class AppointmentsController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private ServicePriceGroupRepository servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        public AppointmentsController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            servicePriceGroupRepository = new ServicePriceGroupRepository(ConnectionString);
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var currentDate = DateTime.Now;
         
           var events = new List<Appointment>
            {
        new Appointment { Name = "Event 1", StartDate = new DateTime(currentDate.Year, currentDate.Month, 5, 12, 0, 0),EndDate = new DateTime(currentDate.Year, currentDate.Month, 6, 12, 0, 0) },
        new Appointment { Name = "Event 2",StartDate = new DateTime(currentDate.Year, currentDate.Month, 15, 12, 0, 0),EndDate = new DateTime(currentDate.Year, currentDate.Month, 16, 12, 0, 0) }

    };


            return View(events);
        }
        public IActionResult Calendar(int? year, int? month)
        {
            var currentDate = DateTime.Now;

            if (year.HasValue && month.HasValue)
            {
                currentDate = new DateTime(year.Value, month.Value, 1);
            }

            ViewBag.CurrentDate = currentDate;
            ViewBag.DaysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

            // Adjust these as necessary for your application
            ViewBag.PreviousMonthYear = currentDate.AddMonths(-1).Year;
            ViewBag.PreviousMonth = currentDate.AddMonths(-1).Month;
            ViewBag.NextMonthYear = currentDate.AddMonths(1).Year;
            ViewBag.NextMonth = currentDate.AddMonths(1).Month;

            // Generate mock data
            var events = new List<Appointment>
            {
        new Appointment { Name = "Event 1", StartDate = new DateTime(currentDate.Year, currentDate.Month, 5, 12, 0, 0),EndDate = new DateTime(currentDate.Year, currentDate.Month, 6, 12, 0, 0) },
        new Appointment { Name = "Event 2",StartDate = new DateTime(currentDate.Year, currentDate.Month, 15, 12, 0, 0),EndDate = new DateTime(currentDate.Year, currentDate.Month, 16, 12, 0, 0) }

    };
            ViewBag.Events = events;

            return View();
        }








    }
    public class Appointment
    {
        public string Name { get; set; }
        public string PatientName { get; set; }
        public string PatientSurname { get; set; }
        public string DoctortName { get; set; }
        public string DoctorSurname { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

}

