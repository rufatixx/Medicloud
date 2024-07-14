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
        var result =  appointmentService.AddAppointment(appointmentDto);
		return RedirectToAction("Index");
	}

    [HttpGet]
    public IActionResult Index()
    {
        var result = appointmentService.GetAllAppointments();
        return View(result.ToList());
    }

    [HttpGet]
    public IActionResult GetAppointmentById([FromQuery] string id)
    {
        var result = appointmentService.GetAppointmentById(id);
        return Ok(result);
    }
}

