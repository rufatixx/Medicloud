
using Medicloud.BLL.DTO;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.OTP;
using Medicloud.BLL.Services.User;
using Medicloud.BLL.Utils;
using Medicloud.DAL.DAO;

using Microsoft.AspNetCore.Mvc;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
	public class RecoveryController : Controller
	{
		private readonly string _connectionString;
		public IConfiguration Configuration;
		private readonly IWebHostEnvironment _hostingEnvironment;
		UserService userService;
		OOrganizationService organizationService;
		private readonly SpecialityService _specialityService;
		private readonly INUserService _userService;
		private readonly HashHelper _hashHelper;
		private readonly IOtpService _otpService;
		public RecoveryController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, SpecialityService specialityService, INUserService userService, IOtpService otpService)
		{
			Configuration = configuration;
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			//userService = new UserService(_connectionString);
			organizationService = new OOrganizationService(_connectionString);
			_specialityService = specialityService;
			_userService = userService;
			_hashHelper = new();
			_otpService = otpService;
		}
		// GET: /<controller>/
		public IActionResult Index()
		{
			return View();
		}
		public IActionResult Step2()
		{
			if (HttpContext.Session.GetString("recoveryOtpId") != null && HttpContext.Session.GetString("recoveryUserId") != null)
			{
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Recovery");
			}
		}

		public IActionResult Step3()
		{
			if (HttpContext.Session.GetString("recoveryOtpId") != null && HttpContext.Session.GetString("recoveryUserId") != null)
			{

				return View();
			}
			else
			{
				return RedirectToAction("Index", "Recovery");
			}
		}

		public IActionResult Success()
		{
			if (HttpContext.Session.GetString("recoveryOtpId") != null && HttpContext.Session.GetString("recoveryUserId") != null)
			{
				HttpContext.Session.Remove("recoveryOtpId");
				HttpContext.Session.Remove("recoveryUserId");
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Recovery");
			}
		}


		[HttpPost]
		public async Task<IActionResult> SendRecoveryOtpForUser(string content, int type)
		{

			try
			{
				if (type == 0 || string.IsNullOrEmpty(content))
				{
					throw new Exception();
				}
				var otpResult = new OtpResult();
				UserDAO existUser = null;
				switch (type)
				{
					case 1:
						existUser = await _userService.GetUserByPhoneNumber(content);
						break;
					case 2:
						existUser = await _userService.GetUserByEmail(content);
						break;
					default:
						break;
				}
				var otpCode = _hashHelper.GenerateOtp();
				if (existUser != null && existUser.isRegistered)
				{

					var existOtp = await _otpService.GetActiveOtpByUserId(existUser.id);
					HttpContext.Session.SetString("recoveryUserId", existUser.id.ToString());
					if (existOtp != null)
					{
						HttpContext.Session.SetString("recoveryOtpId", existOtp.id.ToString());
						otpResult.Success = true;
						otpResult.HasExistOtp = true;
						otpResult.Message = "Aktiv OTP mövcuddur";

					}
					else
					{
						int otpId = await _otpService.CreateOtp(type, existUser.id, otpCode);
						HttpContext.Session.SetString("recoveryOtpId", otpId.ToString());
						otpResult.Success = true;
						otpResult.HasExistOtp = false;
						otpResult.Message = "OTP kod göndərildi";
						Console.WriteLine(otpCode);
					}

				}
				else
				{
					otpResult.Success = false;
					otpResult.HasExistOtp = false;
					otpResult.Message = "İstifadəçi tapılmadı";
				}

				return Json(otpResult);

				//var result = userService.SendRecoveryOtpForUser(phone);
				//if (result.Success)
				//{
				//    HttpContext.Session.SetString("recoveryPhone", phone);
				//    return Json(new { success = true, message = result.Message });
				//}
				//else
				//{
				//    return Json(new { success = false, message = result.Message });
				//}



			}
			catch (Exception ex)
			{
				// Handle the exception and return an appropriate response
				return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
			}



		}

		[HttpPost]
		public async Task<IActionResult>CheckForRecoveryOTP(string otpCode)
		{

			try
			{
				//	var phone = HttpContext.Session.GetString("recoveryPhone");
				//	var result = userService.CheckRecoveryOtpHash(phone, otpCode);

				//	if (result)
				//	{
				//		HttpContext.Session.SetString("recoveryOtpCode", otpCode);
				//		return Json(new { success = true, message = "OK" });
				//	}
				//	else
				//	{
				//		return Json(new { success = false, message = "OTP kod yalnışdır" });
				//	}
				string otpIdString = HttpContext.Session.GetString("recoveryOtpId");
				string userIdString = HttpContext.Session.GetString("recoveryUserId");
	
			if (string.IsNullOrWhiteSpace(otpCode) || string.IsNullOrWhiteSpace(otpIdString) || string.IsNullOrWhiteSpace(userIdString))
			{
				Console.WriteLine($"");
				return Json(new { success = false, message = "Xəta baş verdi" });

			}


			int otpId = int.Parse(otpIdString);
			int userId = int.Parse(userIdString);
			bool result = await _otpService.CheckOtp(_hashHelper.HashOtp(otpCode), userId);


			if (result)
			{
				return Json(new { success = true, message = "OK" });
			}
			else
			{
				return Json(new { success = false, message = "OTP kod yalnışdır" });
			}



			}
			catch (Exception ex)
			{
				// Handle the exception and return an appropriate response
				return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
			}



		}

		[HttpPost]
		public async Task<IActionResult> UpdatePass(string password)
		{
			try
			{
				string otpIdString = HttpContext.Session.GetString("recoveryOtpId");
				string userIdString = HttpContext.Session.GetString("recoveryUserId");
				if (string.IsNullOrEmpty(otpIdString) || string.IsNullOrEmpty(userIdString))
				{
					return BadRequest();

				}
				int otpId = int.Parse(otpIdString);
				int updateUserId = int.Parse(userIdString);

				int userId = await _userService.UpdateUserAsync(new()
				{
					id = updateUserId,
					pwd = _hashHelper.sha256(password),

				});


				return Ok(userId);

			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}

		}



	}
}

