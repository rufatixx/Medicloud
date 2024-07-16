using Medicloud.BLL.Service;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Controllers
{
    public class PatientController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly PatientService _patientService;
        private readonly AppointmentService _appointmentService;

        public PatientController(IConfiguration configuration)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;

            _appointmentService = new AppointmentService(ConnectionString);
			_patientService = new PatientService(ConnectionString);
        }

        [HttpGet]
        public IActionResult GetPatientByName([FromQuery]string search)
        {
            var result = _patientService.GetPatientByName(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")),search);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetPatientById([FromQuery] string id)
        {
            var result = _patientService.GetPatientById(id);
            return Ok(result);
        }
	}
}
