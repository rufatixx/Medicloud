
using Medicloud.BLL.DTO;
using Medicloud.BLL.Service;
using Medicloud.BLL.Services.OTP;
using Medicloud.BLL.Services.User;
using Medicloud.BLL.Utils;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Medicloud.WebUI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Cryptography;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{

	public class RegistrationController : Controller
	{
		private readonly string _connectionString;
		public IConfiguration Configuration;
		private readonly IWebHostEnvironment _hostingEnvironment;
		UserService userService;
		OrganizationService organizationService;
		private readonly SpecialityService _specialityService;
		private readonly INUserService _userService;
		private readonly IOtpService _otpService;
		private readonly HashHelper _hashHelper;
		private readonly CommunicationService _communicationService;
		public RegistrationController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, SpecialityService specialityService, INUserService nuserService, IOtpService otpService)
		{
			Configuration = configuration;
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			userService = new UserService(_connectionString);
			organizationService = new OrganizationService(_connectionString);
			_specialityService = specialityService;
			_userService = nuserService;
			_otpService = otpService;
			_hashHelper = new HashHelper();
			_communicationService=new CommunicationService(_connectionString);
		}
		// GET: /<controller>/
		public async Task<IActionResult> Index()
		{
			var vm = TempData["RegistrationModel"] != null ?
				JsonConvert.DeserializeObject<RegistrationViewModel>(TempData["RegistrationModel"].ToString()) :
				new RegistrationViewModel();

			if (string.IsNullOrWhiteSpace(vm.PhoneNumber) && string.IsNullOrWhiteSpace(vm.Email))
			{
				return RedirectToAction("Index", "Login");

			}
			var otpModel = new OtpViewModel();
			var dao = new UserDAO();
			UserDAO existUser = null;
			switch (vm.Type)
			{
				case 1:
					dao.mobile = vm.PhoneNumber;
					existUser = await _userService.GetUserByPhoneNumber(vm.PhoneNumber);
					break;
				case 2:
					dao.email = vm.Email;
					existUser = await _userService.GetUserByEmail(vm.Email);
					break;
				default:
					break;
			}
			var otpCode = _hashHelper.GenerateOtp();
			if (existUser != null)
			{
				otpModel.UserId = existUser.id;
				var existOtp = await _otpService.GetActiveOtpByUserId(existUser.id);
				if (existOtp != null)
				{
					otpModel.OtpId = existOtp.id;
				}
				else
				{
					int otpId = await _otpService.CreateOtp(vm.Type, existUser.id, otpCode);
					Console.WriteLine(otpCode);
				}

			}
			else
			{
				int newUserId = await _userService.AddUser(dao);
				int otpId = await _otpService.CreateOtp(vm.Type, newUserId, otpCode);
				Console.WriteLine(otpCode);

			}
			return View("Step2",otpModel);
		}


		//public async Task<IActionResult> Step2(OtpViewModel vm)
		//{

		//	if (vm != null)
		//	{
		//		return View(vm);
		//	}
		//	return RedirectToAction("Login/Index");

		//}

		[HttpPost]
		public async Task<IActionResult> Step2(OtpViewModel vm)
		{
			try
			{
				if (vm != null && !string.IsNullOrEmpty(vm.CheckOtpCode))
				{
					bool result = await _otpService.CheckOtp(_hashHelper.HashOtp(vm.CheckOtpCode), vm.UserId);
					if (result)
					{
						return RedirectToAction("Success");
					}
					else
					{
						return View();
					}
				}

				return View();
				//string otpIdString = HttpContext.Session.GetString("registrationOtpId");
				//string userIdString = HttpContext.Session.GetString("newUserId");

				//if (string.IsNullOrWhiteSpace(vm.otpCode) || string.IsNullOrWhiteSpace(otpIdString) || string.IsNullOrWhiteSpace(userIdString))
				//{
				//	Console.WriteLine($"");
				//	return Json(new { success = false, message = "Xəta baş verdi" });

				//}


				//int otpId = int.Parse(otpIdString);
				//int userId = int.Parse(userIdString);


				//if (result)
				//{
				//	return Json(new { success = true, message = "OK" });
				//}
				//else
				//{
				//	return Json(new { success = false, message = "OTP kod yalnışdır" });
				//}



			}
			catch (Exception ex)
			{
				//return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
				Console.WriteLine("Sorğunu emal edərkən xəta baş verdi");
				return View();
			}

		}


		public IActionResult Success()
		{
			if (HttpContext.Session.GetString("registrationOtpId") != null && HttpContext.Session.GetString("newUserId") != null)
			{
				HttpContext.Session.Remove("registrationOtpId");
				HttpContext.Session.Remove("newUserId");
				return View();
			}
			else
			{
				return RedirectToAction("Index", "Registration");
			}
		}

		//public IActionResult Step3()
		//{
		//	//if (HttpContext.Session.GetString("registrationOtpId") != null && HttpContext.Session.GetString("newUserId") != null)
		//	if (vm!=null)
		//	{
		//		//ViewBag.specialities = _specialityService.GetSpecialities();
		//		return View(vm);
		//	}
		//	else
		//	{
		//		return RedirectToAction("Index", "Registration");

		//	}
		//}

		//[HttpPost]
		//public async Task<IActionResult> SendOtpForUserRegistration(string content, int type)
		//{

		//	try
		//	{
		//		if (type == 0 || string.IsNullOrEmpty(content))
		//		{
		//			throw new Exception();
		//		}
		//		var otpResult = new OtpResult();
		//		var dao = new UserDAO();
		//		UserDAO existUser = null;
		//		switch (type)
		//		{
		//			case 1:
		//				dao.mobile = content;
		//				existUser = await _userService.GetUserByPhoneNumber(content);
		//				break;
		//			case 2:
		//				dao.email = content;
		//				existUser = await _userService.GetUserByEmail(content);
		//				break;
		//			default:
		//				break;
		//		}
		//		var otpCode = _hashHelper.GenerateOtp();
		//		if (existUser != null)
		//		{
		//			if (existUser.isRegistered)
		//			{
		//				otpResult.Success = false;
		//				otpResult.HasExistOtp = false;
		//				otpResult.Message = "İstifadəçi mövcuddur";
		//				return Json(otpResult);

		//			}
		//			var existOtp = await _otpService.GetActiveOtpByUserId(existUser.id);
		//			HttpContext.Session.SetString("newUserId", existUser.id.ToString());
		//			if (existOtp != null)
		//			{
		//				HttpContext.Session.SetString("registrationOtpId", existOtp.id.ToString());
		//				otpResult.Success = true;
		//				otpResult.HasExistOtp = true;
		//				otpResult.Message = "Aktiv OTP mövcuddur";
		//			}
		//			else
		//			{
		//				int otpId = await _otpService.CreateOtp(type, existUser.id, otpCode);
		//				HttpContext.Session.SetString("registrationOtpId", otpId.ToString());
		//				otpResult.Success = true;
		//				otpResult.HasExistOtp = false;
		//				otpResult.Message = "OTP kod göndərildi";
		//				Console.WriteLine(otpCode);
		//			}

		//		}
		//		else
		//		{
		//			int newUserId = await _userService.AddUser(dao);
		//			int otpId = await _otpService.CreateOtp(type, newUserId, otpCode);

		//			HttpContext.Session.SetString("newUserId", newUserId.ToString());
		//			HttpContext.Session.SetString("registrationOtpId", otpId.ToString());
		//			otpResult.Success = true;
		//			otpResult.HasExistOtp = false;
		//			otpResult.Message = "OTP kod göndərildi";
		//			Console.WriteLine(otpCode);

		//		}

		//		return Json(otpResult);


		//	}
		//	catch (Exception ex)
		//	{
		//		// Handle the exception and return an appropriate response
		//		return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
		//	}



		//}



			[HttpPost]
		public async Task<IActionResult> AddUser(IFormFile profileImage, string name, string surname, string father, int specialityID, string fin, string bDate, string pwd)
		{

			string otpIdString = HttpContext.Session.GetString("registrationOtpId");
			string userIdString = HttpContext.Session.GetString("newUserId");
			int otpId = int.Parse(otpIdString);
			int newUserId = int.Parse(userIdString);
			if (string.IsNullOrEmpty(otpIdString) || string.IsNullOrEmpty(userIdString))
			{
				return BadRequest();

			}
			string relativeFilePath = "";

			if (profileImage != null && profileImage.Length > 0)
			{
				string fileExtension = Path.GetExtension(profileImage.FileName);

				string fileName = Guid.NewGuid().ToString() + fileExtension;
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "user_images", fileName);
				relativeFilePath = "/user_images/" + fileName;
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await profileImage.CopyToAsync(stream);
				}
			}

			int userId = await _userService.UpdateUserAsync(new()
			{
				id = newUserId,
				name = name,
				surname = surname,
				father = father,
				fin = fin,
				bDate = bDate,
				pwd = _hashHelper.sha256(pwd),
				isRegistered = true

			});
			if (userId > 0)
			{
				return Ok();
			}



			return BadRequest();

		}



	}
}

