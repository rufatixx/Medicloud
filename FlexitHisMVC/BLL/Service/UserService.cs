using System.Text;
using Medicloud.BLL.DTO;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.BLL.Service
{
	public class UserService
	{
		private readonly string _connectionString;
		KassaRepo _kassaRepo;
		UserRepo _userRepository;
		PlanRepository _planRepository;
		UserPlanRepo _userPlanRepo;

		CommunicationService _communicationService;
		OrganizationService _organizationService;
		public UserService(string conString)
		{
			_connectionString = conString;
			_kassaRepo = new KassaRepo(_connectionString);
			_userRepository = new UserRepo(_connectionString);
			_communicationService = new CommunicationService(_connectionString);
			_organizationService = new OrganizationService(conString);
			_planRepository = new PlanRepository(conString);
			_userPlanRepo = new UserPlanRepo(conString);
		}

		public UserDTO SignIn(string mobileNumber, string pass)
		{

			//long formattedPhone = regexPhone(phone);
			UserDTO status = new UserDTO();
			status.personal = new User();
			status.organizations = new List<Organization>();
			status.kassaList = new List<Kassa>();

			try
			{
				UserRepo personalDAO = new UserRepo(_connectionString);
				status.personal = personalDAO.GetUser(mobileNumber, pass);

				OrganizationRepo organizationDAO = new OrganizationRepo(_connectionString);
				status.organizations = organizationDAO.GetOrganizationListByUser(status.personal.ID);

				KassaRepo kassaDAO = new KassaRepo(_connectionString);
				status.kassaList = kassaDAO.GetUserAllowedKassaList(status.personal.ID);


			}
			catch (Exception ex)
			{

				Medicloud.StandardMessages.CallSerilog(ex);
				Console.WriteLine($"Exception: {ex.Message}");
				// status.responseString = $"Exception: {ex.Message}";
			}
			return status;




		}

		static string sha256(string randomString)
		{
			var crypt = new System.Security.Cryptography.SHA256Managed();
			var hash = new System.Text.StringBuilder();
			byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
			foreach (byte theByte in crypto)
			{
				hash.Append(theByte.ToString("x2"));
			}
			return hash.ToString();
		}

		public string createCode(int length)
		{
			// const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			const string valid = "1234567890";
			StringBuilder res = new StringBuilder();
			Random rnd = new Random();
			while (0 < length--)
			{
				res.Append(valid[rnd.Next(valid.Length)]);
			}
			return res.ToString();
		}


		// Method to save data into session instead of cookies
		public bool SaveSession(HttpContext context, string key, string value)
		{
			try
			{
				// Check if HttpContext and Session are available
				if (context != null && context.Session != null)
				{

					// Set the session value
					context.Session.SetString(key, value);
					// Check if the key is "organizationID", indicating a change in organization selection
					if (key == "Medicloud_organizationID")
					{
						ResetUserKassaSession(context, value, context.Session.GetString("Medicloud_userID"));

					}
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				// Log the exception, if logging is set up
				// Console.WriteLine(ex.Message);
				return false;
			}
		}
		public bool ResetUserKassaSession(HttpContext context, string organizationID, string userID)
		{
			try
			{
				var kassaList = _kassaRepo.GetUserKassaByOrganization(Convert.ToInt32(organizationID), Convert.ToInt32(userID));

				// Извлекаем сохраненные значения из сессии
				var selectedKassaName = context.Session.GetString("Medicloud_kassaName");
				var selectedKassaID = context.Session.GetString("Medicloud_kassaID");

				// Проверяем, есть ли касса с таким ID в списке
				var kassaExists = kassaList.Any(k => k.kassaID == Convert.ToInt32(selectedKassaID));

				if (!kassaExists && kassaList.Any())
				{
					// Если такой кассы нет, обновляем сессию на имя первой кассы в списке
					var firstKassaName = kassaList[0].name;
					var firstKassaID = kassaList[0].kassaID.ToString();
					context.Session.SetString("Medicloud_kassaName", firstKassaName);
					context.Session.SetString("Medicloud_kassaID", firstKassaID);
				}
				else if (!kassaList.Any())
				{
					// Если список касс пустой, удаляем значения из сессии
					context.Session.Remove("Medicloud_kassaName");
					context.Session.Remove("Medicloud_kassaID");
				}

				return true;
			}
			catch (Exception ex)
			{
				// Обработка исключения
				return false;
			}
		}

		public bool SaveCookie(HttpContext context, string key, string value)
		{
			try
			{

				// Directly setting the new value for the specified key
				context.Response.Cookies.Append(key, value, new CookieOptions { HttpOnly = true, Secure = true });

				// Check if the key is "organizationID", indicating a change in organization selection
				if (key == "Medicloud_organizationID")
				{
					ResetUserKassaSession(context, value, context.Session.GetString("Medicloud_userID"));

				}
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}

		}
		public bool ResetUserKassaCookies(HttpContext context, string organizationID, string userID)
		{
			try
			{

				var kassaList = _kassaRepo.GetUserKassaByOrganization(Convert.ToInt32(organizationID), Convert.ToInt32(userID));

				var selectedKassaName = context.Request.Cookies["Medicloud_kassaName"];
				var selectedKassaID = context.Request.Cookies["Medicloud_kassaID"];

				// Проверяем, есть ли касса с таким ID в списке
				var kassaExists = kassaList.Any(k => k.kassaID == Convert.ToInt32(selectedKassaID));

				if (!kassaExists && kassaList.Any())
				{
					// Если такой кассы нет, обновляем куки на имя первой кассы в списке
					var firstKassaName = kassaList[0].name;
					var firstKassaID = kassaList[0].kassaID;
					context.Response.Cookies.Append("Medicloud_kassaName", firstKassaName);
					context.Response.Cookies.Append("Medicloud_kassaID", firstKassaID.ToString());


				}
				else if (!kassaList.Any())
				{
					context.Response.Cookies.Delete("Medicloud_kassaName");
					context.Response.Cookies.Delete("Medicloud_kassaID");

				}
				// Add here any other cookies that need to be reset

				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public OtpResult SendOtpForUserRegistration(string phone)
		{
			string randomCode;
			var userStatus = _userRepository.GetUserByPhone(phone);
			var result = new OtpResult();

			try
			{
				randomCode = createCode(4);
				long otpWasSet = 0;

				if (userStatus == null)
				{
					// New user registration
					otpWasSet = _userRepository.InsertUser(
						otp: sha256(randomCode),
						phone: phone
					);
					result.Message = "OTP kod göndərildi.";
				}
				else if (!userStatus.isRegistered)
				{
					if (userStatus.otpSentDate == null || (DateTime.Now - userStatus.otpSentDate.Value).TotalMinutes > 5)
					{
						// Update user registration
						otpWasSet = _userRepository.UpdateUser(userStatus.ID, otp: sha256(randomCode));
						result.Message = "İstifadəçi qeydiyyatı yeniləndi. OTP göndərildi.";
					}
					else
					{
						var nextPossibleTime = userStatus.otpSentDate.Value.AddMinutes(5);
						var remainingTime = nextPossibleTime - DateTime.Now;
						result.Success = false;
						result.Message = $"Son 5 dəqiqə ərzində OTP göndərilmişdir. Növbəti mümkün göndərmə vaxtı: {nextPossibleTime.ToString("HH:mm:ss")}, {remainingTime.Minutes} dəqiqə {remainingTime.Seconds} saniyə sonra yenidən cəhd edin.";
						return result;
					}
				}
				else
				{
					result.Success = false;
					result.Message = "İstifadəçi artıq qeydiyyatdan keçmişdir.";
					return result;
				}
				if (otpWasSet > 0)
				{
					// Optionally send the SMS
					//_communicationService.sendSMS($"OTP: {randomCode}", phone);
					Console.WriteLine("OTP:" + randomCode);
					result.Success = true;
					result.Message = "OTP kod göndərildi";
				}
				else
				{
					result.Success = false;
					result.Message = "Failed to set OTP.";
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				// elangoAPI.StandardMessages.CallSerilog(ex);
				result.Success = false;
				result.Message = "Server error: " + ex.Message;
			}

			return result;
		}

		public OtpResult SendRecoveryOtpForUser(string phone)
		{
			var result = new OtpResult();
			var userStatus = _userRepository.GetUserByPhone(phone);

			if (userStatus == null)
			{
				result.Message = "Hesabınız tapılmadı.";
				return result;
			}

			if (!userStatus.isRegistered)
			{
				result.Success = false;
				result.Message = "Hesabınız tapılmadı.";
				return result;
			}

			try
			{
				if (userStatus.recovery_otp_send_date == null ||
					(DateTime.Now - userStatus.recovery_otp_send_date.Value).TotalMinutes > 5)
				{
					string randomCode = createCode(4);
					long otpWasSet = _userRepository.UpdateUser(userStatus.ID, recoveryOtp: sha256(randomCode), recoveryOtpSendDate: DateTime.Now);

					if (otpWasSet > 0)
					{
						// Optionally send the SMS
						//_communicationService.sendSMS($"OTP: {randomCode}", phone);
						Console.WriteLine("OTP:" + randomCode);
						result.Success = true;
						result.Message = "OTP successfully sent.";
					}
					else
					{
						result.Success = false;
						result.Message = "Failed to set OTP.";
					}
				}
				else
				{
					var nextPossibleTime = userStatus.otpSentDate.Value.AddMinutes(5);
					var remainingTime = nextPossibleTime - DateTime.Now;
					result.Success = false;
					result.Message = $"Son 5 dəqiqə ərzində OTP göndərilmişdir. Növbəti mümkün göndərmə vaxtı: {nextPossibleTime.ToString("HH:mm:ss")}, {remainingTime.Minutes} dəqiqə {remainingTime.Seconds} saniyə sonra yenidən cəhd edin.";
				}
			}
			catch (Exception ex)
			{
				// Log the exception
				// elangoAPI.StandardMessages.CallSerilog(ex);
				result.Success = false;
				result.Message = "Server error: " + ex.Message;
			}

			return result;
		}


		public bool AddUser(string phone, string name, string surname, string father, int specialityID, string fin, string bDate, string pwd, string organizationName, int planID)
		{

			var user = _userRepository.GetUserByPhone(phone);
			try
			{
				var updated = _userRepository.UpdateUser(user.ID, name, surname, father, specialityID, fin: fin, bDate: bDate, password: sha256(pwd), isActive: 1, isUser: 1, isRegistered: 1);

				var orgID = _organizationService.AddOrganizationToNewUser(user.ID, organizationName);
				var kassaID = _kassaRepo.CreateKassa($"{organizationName} (Kassa)", orgID);
				var kasaUserRelID = _kassaRepo.InsertKassaToUser(user.ID, kassaID, false, true);
				var plan = _planRepository.GetById(planID);
				_userPlanRepo.AddUserPlan(user.ID, plan.id, plan.duration, true);

				if (updated > 0 && orgID > 0)
				{
					return true;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return false;

		}

		public bool UpdatePassword(string otpCode, string phone, string pwd)
		{
			if (CheckRecoveryOtpHash(phone, otpCode))
			{
				var user = _userRepository.GetUserByPhone(phone);

				if (user == null)
				{
					throw new Exception("Hesab tapılmadı.");
				}

				try
				{
					if (user != null)
					{

						var updated = _userRepository.UpdateUser(user.ID, password: sha256(pwd));
						if (updated <= 0)
						{
							throw new Exception("Failed to update password.");
						}

						return true;
					}

				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					throw new Exception("Hesab mövcud deyil.");
				}
			}

			return false;

		}


		public bool CheckOtpHash(string phone, string providedOtp)
		{
			if (!string.IsNullOrEmpty(phone) || !string.IsNullOrEmpty(providedOtp))
			{
				var providedOtpHash = sha256(providedOtp);

				var otpData = _userRepository.GetOtpData(phone);

				if (otpData == providedOtpHash)
				{
					return true;
				}
			}

			return false;
		}

		public bool CheckRecoveryOtpHash(string phone, string providedOtp)
		{
			if (!string.IsNullOrEmpty(phone) || !string.IsNullOrEmpty(providedOtp))
			{
				var providedOtpHash = sha256(providedOtp);

				var otpData = _userRepository.GetRecoveryOtpData(phone);

				if (otpData == providedOtpHash)
				{
					return true;
				}
			}

			return false;
		}

		public long InsertUser(string name = "", string surname = "",
	  string father = "", int specialityID = 0, string passportSerialNum = "",
	  string fin = "", string phone = "", string email = "",
	  string bDate = "", string username = "", string pwd = "",
	  int isUser = 0, int isDr = 0, int isAdmin = 0,
	  int isActive = 0, string otp = "")
		{

			return _userRepository.InsertUser(name, surname,
		   father, specialityID, passportSerialNum, fin, phone, email,
			bDate, username, pwd,
			isUser, isDr, isAdmin,
			isActive);

		}


		public long UpdateUser(int userID = 0, string name = "", string surname = "", string father = "",
			int specialityID = 0, string passportSerialNum = "", string fin = "",
			string mobile = "", string email = "", string bDate = "",
			string username = "", int isUser = 0, int isDr = 0, int isActive = 0,
			int isAdmin = 0, string otp = "", string recoveryOtp = "", DateTime? recoveryOtpSendDate = null, string password = "", int isRegistered = 0)
		{

			return _userRepository.UpdateUser(userID, name, surname, father,
			 specialityID, passportSerialNum, fin,
			 mobile, email, bDate,
			 username, isUser, isDr, isActive,
			 isAdmin, otp, recoveryOtp, recoveryOtpSendDate, password, isRegistered);

		}



		public User GetUserById(int id)
		{
			return _userRepository.GetUserByID(id);
		}

	}


}




