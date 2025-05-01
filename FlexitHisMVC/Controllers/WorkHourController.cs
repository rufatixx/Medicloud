using Medicloud.BLL.Models;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.Abstract;
using Medicloud.BLL.Services.Concrete;
using Medicloud.BLL.Services.WorkHour;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Linq;

namespace Medicloud.Controllers
{
	public class WorkHourController : Controller
	{
		private readonly string ConnectionString;
		public IConfiguration Configuration;
		private readonly IWorkHourService _workHourService;
		private readonly AppointmentService _appointmentService;
		private readonly IPatientCardService _patientCardService;
        public WorkHourController(IWorkHourService workHourService, IConfiguration configuration, IPatientCardService patientCardService)
        {
            _workHourService = workHourService;
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _appointmentService = new AppointmentService(ConnectionString);
            _patientCardService=patientCardService;
        }

        //public IActionResult Index()
        //{
        //	return View();
        //}
        [HttpGet]
		public async Task<IActionResult> GetUserWorkHours(int userId,DateTime selectedDay)
		{

			int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			var userWorkHours = await _workHourService.GetOrganizationUserWorkHours(userId, organizationId);
			int dayWeek = (int)selectedDay.DayOfWeek;
			if(dayWeek==0)dayWeek = 7;
            var dayWorkHours = userWorkHours.FirstOrDefault(w => w.dayOfWeek == dayWeek);
			//var reserves= _appointmentService.GetAppointmentByDate(selectedDay,organizationId,userId);
			var reserves=await _patientCardService.GetPatientsCardsByDate(selectedDay,organizationId,userId);
			if(reserves !=null && reserves.Count > 0)
			{
				dayWorkHours.Reserves = reserves.Select(r => new BreakDTO()
				{
					start = r.startDate?.TimeOfDay,
					end = r.endDate?.TimeOfDay,
					id = (int)r.id,
				}).ToList();
			}

			return Ok(dayWorkHours);
		}
	}
}
