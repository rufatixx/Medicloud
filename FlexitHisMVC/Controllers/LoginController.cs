using System.Security.Claims;
using System.Xml;
using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Organization;
using Medicloud.BLL.Services;
using Medicloud.DAL.Repository.Role;
using Medicloud.DAL.Repository.UserPlan;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

	public class LoginController : Controller
	{

		private readonly string _connectionString;
		public IConfiguration Configuration;
		private readonly IWebHostEnvironment _hostingEnvironment;
		IUserService _userService;
		private readonly IRoleRepository _roleRepository;
		private readonly IUserPlanRepo _userPlanRepo;
		private readonly IOrganizationService _organizationService;
		public LoginController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IUserService userService, IRoleRepository roleRepository, IUserPlanRepo userPlanRepo, IOrganizationService organizationService)
		{
			Configuration = configuration;
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			_userService = userService;
			_roleRepository = roleRepository;
			_userPlanRepo = userPlanRepo;
			_organizationService = organizationService;
		}


		// GET: /<controller>/
		public IActionResult Index()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return View();

		}

		[HttpPost("[controller]/[action]")]
		public async Task<IActionResult> SignInAsync(string content, string pass, int type)
		{

			var obj = await _userService.SignIn(content, pass, type);

			if (obj.personal.ID > 0)
			{
				var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, $"{obj.personal.name} {obj.personal.surname}"),
				new Claim("ID",obj.personal.ID.ToString())
                // Add additional claims as needed
            };

				var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

				var authProperties = new AuthenticationProperties
				{
					IsPersistent = true, // Remember the user across sessions
				};

				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


				ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>();
				response.data = new List<UserDTO>();
				response.data.Add(obj);


				//HttpContext.Session.SetInt32("userid", obj.personal.ID);



				var currentOrganization = obj.organizations[0];

				_userService.SaveSession(HttpContext, "Medicloud_userID", obj.personal.ID.ToString());
				_userService.SaveSession(HttpContext, "Medicloud_organizationID", currentOrganization.organizationID.ToString());
				_userService.SaveSession(HttpContext, "Medicloud_organizationName", currentOrganization.organizationName.ToString());
				_userService.SaveSession(HttpContext, "Medicloud_organizationOwnerId", currentOrganization.ownerId.ToString());
				var plan = await _userPlanRepo.GetPlansByUserId(currentOrganization.ownerId);
				if (plan != null)
				{
					_userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", plan.expire_date.ToString());
				}
				//if (currentOrganization?.Roles != null)
				//{
				//	var rolesIds = currentOrganization.Roles.Select(r => r.id);
				//	string rolesIdsString = string.Join(",", rolesIds);
				//	_userService.SaveSession(HttpContext, "Medicloud_UserRoles", rolesIdsString);
				//}


				//userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", DateTime.Now.AddDays(10).ToString());
				return Ok(response);
			}
			else
			{
				return Unauthorized();
			}

		}




		[HttpPost("[controller]/[action]")]
		public async Task<IActionResult> SwitchOrganization(int organizationId)
		{
			int userId = int.Parse(HttpContext.Session.GetString("Medicloud_userID"));

			var organization =await _organizationService.GetOrganizationById(organizationId);
			if (organization != null)
			{
				_userService.SaveSession(HttpContext, "Medicloud_organizationID", organization.organizationID.ToString());
				_userService.SaveSession(HttpContext, "Medicloud_organizationName", organization.organizationName.ToString());
				_userService.SaveSession(HttpContext, "Medicloud_organizationOwnerId", organization.ownerId.ToString());
				return Ok();

			}
			return BadRequest();

		}


		[HttpPost("[controller]/[action]")]
		public async Task<IActionResult> LogoutAsync()
		{

			foreach (var cookie in Request.Cookies.Keys)
			{
				Response.Cookies.Delete(cookie);
			}

			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			HttpContext.Session.Clear();

			return Ok();

		}
	}
}

