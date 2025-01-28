using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Medicloud.BLL.DTO;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.User;
using Medicloud.DAL.DAO;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Medicloud.WebUI.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

	public class LoginController : Controller
	{

		private readonly string _connectionString;
		public IConfiguration Configuration;
		private readonly IWebHostEnvironment _hostingEnvironment;
		UserService userService;
		private readonly INUserService _nUserService;
		public LoginController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, INUserService userService)
		{
			Configuration = configuration;
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;
			//userService = new UserService(_connectionString);
			_nUserService = userService;
		}


		// GET: /<controller>/
		public IActionResult Index(LoginViewModel vm=null)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");
			}

			return View(vm);

		}

		//    [HttpPost("[controller]/[action]")]
		//    public async Task<IActionResult> SignInAsync(string mobileNumber, string pass)
		//    {
		//        UserService login = new UserService(_connectionString);
		//        var obj = login.SignIn(mobileNumber, pass);

		//        if (obj.personal.ID > 0)
		//        {
		//            var claims = new List<Claim>
		//        {
		//            new Claim(ClaimTypes.Name, $"{obj.personal.name} {obj.personal.surname}"),
		//            new Claim("ID",obj.personal.ID.ToString())
		//            // Add additional claims as needed
		//        };

		//            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		//            var authProperties = new AuthenticationProperties
		//            {
		//                IsPersistent = true, // Remember the user across sessions
		//            };

		//            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


		//            ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>();
		//            response.data = new List<UserDTO>();
		//            response.data.Add(obj);


		//            //HttpContext.Session.SetInt32("userid", obj.personal.ID);




		//            userService.SaveSession(HttpContext, "Medicloud_userID", obj.personal.ID.ToString());
		//if (obj.organizations!=null && obj.organizations.Count>0)
		//{
		//	userService.SaveSession(HttpContext, "Medicloud_organizationID", obj.organizations[0].organizationID.ToString());
		//	userService.SaveSession(HttpContext, "Medicloud_organizationName", obj.organizations[0].organizationName.ToString());

		//}
		////userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", obj.personal.subscription_expire_date.ToString());
		//userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", DateTime.Now.AddDays(10).ToString());
		//            return Ok(response);
		//        }
		//        else
		//        {
		//            return Unauthorized();
		//        }

		//    }


		[HttpPost("[controller]/[action]")]
		public async Task<IActionResult> SignIn(LoginViewModel vm)
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");

			}
			string contact=vm.Type==1?vm.PhoneNumber:vm.Email;

			if (string.IsNullOrEmpty(vm.Password))
			{
				var otpResult = new OtpResult();
				UserDAO existUser = null;
				switch (vm.Type)
				{
					case 1:
						existUser = await _nUserService.GetUserByPhoneNumber(contact);
						break;
					case 2:
						existUser = await _nUserService.GetUserByEmail(contact);
						break;
					default:
						break;
				}
				if (existUser != null && existUser.isRegistered==true)
				{
					vm.UserExist = true;
					return View("Index", vm);

				}
				else
				{
					var registerVM = new RegistrationViewModel
					{
						Email = vm.Email,
						Type = vm.Type,
						PhoneNumber = vm.PhoneNumber,
					};
					TempData["RegistrationModel"] = JsonConvert.SerializeObject(registerVM);
					return RedirectToAction("Index", "Registration");
				}
			}
			var user = await _nUserService.SignInAsync(contact, vm.Type, vm.Password);
			if (user == null)
			{
				vm.UserExist=false;
				vm.Password = "";
				return View("Index", vm);
			}

			var tokenHandler = new JwtSecurityTokenHandler();

			var key = Encoding.ASCII.GetBytes(Configuration.GetSection("JwtSettings:SecretKey").Value);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[] {
					new Claim(ClaimTypes.Name,$"{user.name} {user.surname}"),
					new Claim(ClaimTypes.NameIdentifier,user.id.ToString())
				}),
				Expires = DateTime.Now.AddMinutes(30),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);
			Response.Cookies.Append("JwtToken", tokenString, new CookieOptions
			{
				HttpOnly = true,
				Secure=true,
				SameSite = SameSiteMode.Strict, 
				Expires = DateTime.Now.AddMinutes(30)
			});

			return RedirectToAction("Index", "Home");

		}


		[HttpGet("[controller]/[action]")]
		public async Task<IActionResult> PostLogin()
		{
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Home");

			}
			else
			{
				return Unauthorized();

			}
		}


		//[HttpPost("[controller]/[action]")]
		//public async Task<IActionResult> SignInAsync(string contact, int contactType, string pass)
		//{
		//	var user = await _nUserService.SignInAsync(contact, contactType, pass);
		//	if (user == null)
		//	{
		//		return Unauthorized();
		//	}

		//	var claims = new List<Claim>
		//		{
		//			new Claim(ClaimTypes.Name, $"{user.name} {user.surname}"),
		//			new Claim("ID",user.id.ToString())
		//                  // Add additional claims as needed
		//              };

		//	var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

		//	var authProperties = new AuthenticationProperties
		//	{
		//		IsPersistent = true, // Remember the user across sessions
		//	};

		//	await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


		//	ResponseDTO<UserDTO> response = new ResponseDTO<UserDTO>();
		//	response.data = new List<UserDTO>();
		//	//response.data.Add(obj);


		//	//HttpContext.Session.SetInt32("userid", obj.personal.ID);

		//	HttpContext.Session.SetString("Medicloud_UserPlanExpireDate", DateTime.Now.AddDays(10).ToString());


		//	//userService.SaveSession(HttpContext, "Medicloud_userID", user.id.ToString());
		//	//if (obj.organizations!=null && obj.organizations.Count>0)
		//	//{
		//	//    userService.SaveSession(HttpContext, "Medicloud_organizationID", obj.organizations[0].organizationID.ToString());
		//	//    userService.SaveSession(HttpContext, "Medicloud_organizationName", obj.organizations[0].organizationName.ToString());

		//	//}
		//	//userService.SaveSession(HttpContext, "Medicloud_UserPlanExpireDate", obj.personal.subscription_expire_date.ToString());
		//	//userService.SaveSession(HttpContext, );
		//	return Ok();
		//	//}
		//	//else
		//	//{
		//	//    return Unauthorized();
		//	//}

		//}

		[HttpPost("[controller]/[action]")]
		public async Task<IActionResult> CheckUserExist(string contact,int contactType)
		{
			if (contactType == 0 || string.IsNullOrEmpty(contact))
			{
				throw new Exception();
			}
			var otpResult = new OtpResult();
			UserDAO existUser = null;
			switch (contactType)
			{
				case 1:
					existUser = await _nUserService.GetUserByPhoneNumber(contact);
					break;
				case 2:
					existUser = await _nUserService.GetUserByEmail(contact);
					break;
				default:
					break;
			}
			bool result = existUser != null;
			return Ok(result);

		}

		[HttpGet("[controller]/[action]")]
		public async Task<IActionResult> LogOut()
		{


			Response.Cookies.Delete("JwtToken");  
			HttpContext.Session.Clear();


			return RedirectToAction("Index");

			//foreach (var cookie in Request.Cookies.Keys)
			//{
			//	Response.Cookies.Delete(cookie);
			//}

			//await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			//return Ok();

		}
	}
}

