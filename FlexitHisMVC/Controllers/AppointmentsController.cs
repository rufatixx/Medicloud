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
}

