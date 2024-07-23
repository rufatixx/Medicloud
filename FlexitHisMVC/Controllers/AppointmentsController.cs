using Medicloud.BLL.Models;
using Medicloud.BLL.Service;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Controllers;


public class AppointmentsController : Controller
{
    private readonly string ConnectionString;
    public IConfiguration Configuration;
    private readonly AppointmentService appointmentService;

    
    public AppointmentsController(IConfiguration configuration)
    {
        Configuration = configuration;
        ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;

        appointmentService = new AppointmentService(ConnectionString);
    }

    [HttpPost]
    public IActionResult AddAppointment(AddAppointmentDto appointmentDto)
    {
        appointmentDto.OrganizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));
		var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
		appointmentDto.UserId = userID;
		if (appointmentDto.Id > 0)
		{
			appointmentService.UpdateAppointment(appointmentDto);
		} else
		{
			appointmentService.AddAppointment(appointmentDto);
		}
		return RedirectToAction("Index");
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

