using Medicloud.BLL.Models;
using Medicloud.BLL.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace Medicloud.Controllers;

[Authorize]
public class AppointmentsController : Controller
{
    private readonly string ConnectionString;
    public IConfiguration Configuration;
    private readonly AppointmentService appointmentService;
	private readonly CommunicationService communicationService;
    private readonly UserService _userService;
    public AppointmentsController(IConfiguration configuration)
    {
        Configuration = configuration;
        ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;

        appointmentService = new AppointmentService(ConnectionString);
		communicationService = new CommunicationService(ConnectionString);
		_userService = new UserService(ConnectionString);
    }

    [HttpPost]
    public IActionResult AddAppointment(AddAppointmentDto appointmentDto)
    {
	    string referer = Request.Headers["Referer"].ToString();
	    
        appointmentDto.OrganizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
        appointmentDto.UserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
		var user=_userService.GetUserById(appointmentDto.UserId);
		
		if (appointmentDto.Id > 0)
		{
			appointmentService.UpdateAppointment(appointmentDto);
		} else
		{
			appointmentService.AddAppointment(appointmentDto);

			if (user != null && appointmentDto.PhoneNumber > 0)
			{
				var userTel = user.mobile;
				var patientTel = appointmentDto.PhoneNumber;
				var dateTime = appointmentDto.MeetingDate;
				var date = dateTime.ToString("yyyy-MM-dd");
				var time = $"{appointmentDto.Time.Hours:D2}:{appointmentDto.Time.Minutes:D2}";
				var message = $"Hormetli pasient,Sizin Dr {user.name} ile {date} saat {time} randevunuz var.Etrafli: {userTel}";
				communicationService.sendSMS(message, patientTel.ToString());
			}

		}

		return referer.Contains("Calendar") ? RedirectToAction("Index", "Calendar") : RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Index([FromQuery] int pageNumber=1)
    {
        var result = appointmentService.GetAllAppointments(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), null, pageNumber);
        return View(result);
    }

    [HttpGet]
    public IActionResult GetAppointmentById([FromQuery] string id)
    {
        var result = appointmentService.GetAppointmentById(id);
        return Ok(result);
    }

    [HttpDelete]
    public IActionResult DeleteAppointmentById([FromQuery] string id)
    {
        var result = appointmentService.DeleteAppointment(id);
        return Ok(result);
    }

	[HttpGet]
	public IActionResult GetAppointmentsByRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
	{
		var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
		var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
		var result = appointmentService.GetAppointmentsByRange(startDate, endDate, userID, organizationID);
		return Ok(result);
	}

	[HttpGet]
	public IActionResult GetAppointmentByDate([FromQuery] DateTime date)
	{
		var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
		var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
		var result = appointmentService.GetAppointmentByDate(date, userID, organizationID);
		return Ok(result);
	}
}

