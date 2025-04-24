using Medicloud.BLL.Models;
using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Communication;
using Medicloud.BLL.Services;
using Medicloud.DAL.Repository.Role;
using Medicloud.Models;
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
    private readonly ICommunicationService _communicationService;
    private readonly IUserService _userService;
    private readonly IRoleRepository _roleRepository;
    public AppointmentsController(IConfiguration configuration, IRoleRepository roleRepository, ICommunicationService communicationService, IUserService userService)
    {
        Configuration = configuration;
        ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;

        appointmentService = new AppointmentService(ConnectionString);
        _communicationService = communicationService;
        _userService = userService;
        _roleRepository = roleRepository;
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointment(AddAppointmentDto appointmentDto)
    {
        // Store referrer (to know where to redirect back)
        string referer = Request.Headers["Referer"].ToString();
		// Retrieve the ID claim of the currently logged-in user
		int claimsUserId = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");

		// Retrieve the organization ID from session
		int organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

		// Retrieve roles for this user in the current organization
		var userRoles = await _roleRepository.GetUserRoles(organizationID, claimsUserId);
		var roleIds = userRoles.Select(r => r.id).ToList();

		// Check for relevant roles
		bool hasRole4 = roleIds.Contains(4);
		bool hasRole7 = roleIds.Contains(7);

		// Always assign the organization ID
		appointmentDto.OrganizationID = organizationID;

		// Enforce your specific logic:
		if (hasRole4 && hasRole7)
		{
			// If user has both role 4 and 7,
			// keep the incoming UserId if it’s provided (> 0).
			// If no valid UserId is provided, you can decide to default to claimsUserId or handle it.
			if (appointmentDto.UserId <= 0)
			{
				appointmentDto.UserId = claimsUserId;
			}
			// Else: keep appointmentDto.UserId as passed in.
		}
		else if (hasRole7)
		{
			// If user only has role 7, keep the incoming UserId from the DTO
			// (assuming the caller sets it).
			// Validate if needed; for instance, ensure appointmentDto.UserId > 0.
			if (appointmentDto.UserId <= 0)
			{
				return BadRequest("A valid 'UserId' must be provided when you have only role 7.");
			}
		}
		else if (hasRole4)
		{
			// If user only has role 4, override the DTO’s UserId with the claims user ID
			appointmentDto.UserId = claimsUserId;
		}
		else
		{
			// If the user has neither role 4 nor role 7, do not allow creation
			return Forbid("You are not authorized to create appointments.");
		}

		// Retrieve user details (depending on final assigned appointmentDto.UserId)
		var user = _userService.GetUserById(appointmentDto.UserId);

		// Handle "update vs. add" flow
		// (Replace with your actual create/update service calls)
		if (appointmentDto.Id > 0)
		{
			appointmentService.UpdateAppointment(appointmentDto);
		}
		else
		{
			appointmentService.AddAppointment(appointmentDto);

			// Optionally send SMS to the patient if phone number is specified
			if (user != null && appointmentDto.PhoneNumber > 0)
			{
				var userTel = user.mobile; // Doctor's mobile number
				var patientTel = appointmentDto.PhoneNumber;
				var dateTime = appointmentDto.MeetingDate; // The date/time of appointment
				var date = dateTime.ToString("yyyy-MM-dd");
				string startTimeString = appointmentDto.StartTime.ToString(@"hh\:mm");
				string endTimeString = appointmentDto.EndTime.ToString(@"hh\:mm");

				var time = $"{startTimeString}:{endTimeString}";

				var message = $"Hormetli pasient, Sizin Dr {user.name} ile {date} saat {time} randevunuz var. Etrafli: {userTel}";
				//_communicationService.sendSMS(message, patientTel.ToString());
			}
		}

		// Redirect logic
		if (referer.Contains("Calendar"))
            return RedirectToAction("Index", "Calendar");
        else
            return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int pageNumber = 1)
    {
        var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
        var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
        var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
        var roles = userRoles.Select(r => r.id);
        AppointmentPagedResult result = null;
        if (roles.Contains(7))
        {
            ViewBag.allStaff = _userService.GetUserList(organizationID);
            result = appointmentService.GetAllAppointments(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), null, 0, pageNumber: pageNumber);

        }
        else if (roles.Contains(4))
        {
            result = appointmentService.GetAllAppointments(Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID")), null,userID, pageNumber: pageNumber);

        }



        return View(result);
    }

    [HttpGet]
    public IActionResult GetAppointmentById([FromQuery] string id)
    {
		Console.WriteLine(id);
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
    public async Task<IActionResult> GetAppointmentsByRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
        var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
        var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
        var roles = userRoles.Select(r => r.id);

        List<AppointmentViewModel> result = new List<AppointmentViewModel>();

        if (roles.Contains(7))
        {
            result = appointmentService.GetAppointmentsByRange(startDate, endDate, organizationID);

        }
        else if (roles.Contains(4))
        {
            result = appointmentService.GetAppointmentsByRange(startDate, endDate, organizationID, userID);
        }


        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAppointmentByDate([FromQuery] DateTime date)
    {

        var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
        var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
        var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
        var roles = userRoles.Select(r => r.id);

        List<AppointmentViewModel> result = new List<AppointmentViewModel>();

        if (roles.Contains(7))
        {
            result = appointmentService.GetAppointmentByDate(date, organizationID);

        }
        else if (roles.Contains(4))
        {
            result = appointmentService.GetAppointmentByDate(date, organizationID, userID);
        }


        return Ok(result);





    }
}

