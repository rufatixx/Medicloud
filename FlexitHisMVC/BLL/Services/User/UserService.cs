using System.Text;
using Medicloud.BLL.DTO;
using Medicloud.BLL.Service.Communication;
using Medicloud.BLL.Service.Organization;
using Medicloud.BLL.Services;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Kassa;
using Medicloud.DAL.Repository.Plan;
using Medicloud.DAL.Repository.Role;
using Medicloud.DAL.Repository.UserPlan;

using Medicloud.DAL.Repository.Users;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;


namespace Medicloud.BLL.Service
{
	public class UserService:IUserService
	{
		private readonly string _connectionString;
		//KassaRepo _kassaRepo;
		private readonly IKassaRepo _kassaRepo;
		IUserRepository _userRepository;

        IUserPlanRepo _userPlanRepo;
		ICommunicationService _communicationService;
        IOrganizationService _organizationService;
        IPlanRepository _planRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IRoleRepository _roleRepository;
		public UserService(IKassaRepo kassaRepo, ICommunicationService communicationService, IUserPlanRepo userPlanRepo, IOrganizationService organizationService, IUserRepository userRepository, IPlanRepository planRepository, IUnitOfWork unitOfWork, IRoleRepository roleRepository)
		{



			_communicationService = communicationService;
			_userRepository = userRepository;
			_organizationService = organizationService;
			_planRepository = planRepository;
			_userPlanRepo = userPlanRepo;
			_unitOfWork = unitOfWork;
			_kassaRepo = kassaRepo;
			_roleRepository = roleRepository;
			//_nUserRepository = new UserRepository()
		}

		public async Task<UserDTO> SignIn(string content, string pass,int type)
		{

			//long formattedPhone = regexPhone(phone);
			UserDTO status = new UserDTO();
			status.personal = new UserDAO();
			status.organizations = new List<OrganizationDAO>();
			status.kassaList = new List<KassaDAO>();

			try
			{
				
				status.personal = _userRepository.GetUser(content, pass,type);
				//status.personal = await _nUserRepository.GetUser(mobileNumber, pass);


				status.organizations = _organizationService.GetOrganizationsByUser(status.personal.ID);

				foreach (var item in status.organizations)
				{
					var itemroles = await _roleRepository.GetUserRoles(item.organizationID, status.personal.ID);
					item.Roles = itemroles;
				}
				status.kassaList = _kassaRepo.GetUserAllowedKassaList(status.personal.ID);


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

		public async Task<OtpResult> SendOtpForUserRegistration(string content,int type)
		{
			UserDAO userStatus = null;
            var result = new OtpResult();

            string randomCode;
			if (type==1)
			{
                userStatus = _userRepository.GetUserByPhone(content);

            }
			else if(type==2)
			{
                userStatus = _userRepository.GetUserByEmail(content);

            }
			else
			{
                result.Success = false;
                result.Message = "Xəta baş verdi. Zəhmət olmasa biraz sonra təkrar cəhd edin";
                return result;
            }

			try
			{
				randomCode = createCode(4);
				long otpWasSet = 0;

				if (userStatus == null)
				{
					if (type==1)
					{
						// New user registration
						otpWasSet = _userRepository.InsertUser(
							otp: sha256(randomCode),
							phone: content
						);
						result.Message = "OTP kod göndərildi.";
					}
					else if (type==2)
					{
						// New user registration
						otpWasSet = _userRepository.InsertUser(
							otp: sha256(randomCode),
							email: content
						);
						result.Message = "OTP kod göndərildi.";
					}
					else
					{
                        result.Success = false;
                        result.Message = "Xəta baş verdi. Zəhmət olmasa biraz sonra təkrar cəhd edin";
                        return result;
                    }

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
					if (type==1)
					{
						//_communicationService.sendSMS($"OTP: {randomCode}", content);
						Console.WriteLine("OTP:" + randomCode);
                        result.Success = true;
                        result.Message = "OTP kod göndərildi";
                    }
					else if(type==2)
					{
						//await _communicationService.sendMail($"OTP: {randomCode}", content);
						Console.WriteLine("OTP:" + randomCode);
                        result.Success = true;
                        result.Message = "OTP kod göndərildi";
                    }
					else
					{
                        result.Success = false;
                        result.Message = "Xəta baş verdi. Zəhmət olmasa biraz sonra təkrar cəhd edin";
                        return result;
                    }

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

		public async Task<OtpResult> SendRecoveryOtpForUser(string content,int type)
		{
            UserDAO userStatus = null;
            var result = new OtpResult();
            if (type==1)
            {
                userStatus = _userRepository.GetUserByPhone(content);

            }
            else if (type==2)
            {
                userStatus = _userRepository.GetUserByEmail(content);

            }

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
                        //// Optionally send the SMS
                        ////_communicationService.sendSMS($"OTP: {randomCode}", phone);
                        //Console.WriteLine("OTP:" + randomCode);
                        //result.Success = true;
                        //result.Message = "OTP successfully sent.";

                        if (type==1)
                        {
							_communicationService.sendSMS($"OTP: {randomCode}", content);
							Console.WriteLine("OTP:" + randomCode);
                            result.Success = true;
                            result.Message = "OTP kod göndərildi";
                        }
                        else if (type==2)
                        {
							await _communicationService.sendMail($"OTP: {randomCode}", content);
							Console.WriteLine("OTP:" + randomCode);
                            result.Success = true;
                            result.Message = "OTP kod göndərildi";
                        }
                        else
                        {
                            result.Success = false;
                            result.Message = "Xəta baş verdi. Zəhmət olmasa biraz sonra təkrar cəhd edin";
                            return result;
                        }
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


		public async Task<(int userId,long organizationId)> AddUser(string phone,string email, string name, string surname, string father, int specialityID, string fin, string bDate, string pwd, string organizationName, int planID,string imagePath)
		{
			UserDAO user = null;
			if (!string.IsNullOrEmpty(phone))
			{
                user = _userRepository.GetUserByPhone(phone);

            }
			else if (!string.IsNullOrEmpty(email))
			{
                user = _userRepository.GetUserByEmail(email);

            }

            try
			{
				var updated = _userRepository.UpdateUser(user.ID, name, surname, father, specialityID, fin: fin, bDate: bDate, password: sha256(pwd), isActive: 1, isUser: 1, isRegistered: 1,isAdmin:1,imagePath:imagePath);

				var orgID = _organizationService.AddOrganizationToNewUser(user.ID, organizationName);
				var kassaID = _kassaRepo.CreateKassa($"{organizationName} (Kassa)", orgID);
				var kasaUserRelID = _kassaRepo.InsertKassaToUser(user.ID, kassaID, false, true);
				var plan = _planRepository.GetById(planID);
				_userPlanRepo.AddUserPlan(user.ID, plan.id, plan.duration, true); 

				if (updated > 0 && orgID > 0)
				{
					return (user.ID,orgID);
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			return (0,0);

		}

        public bool AddOrganizationAndKassaToExistingUser(int userId, string organizationName, string kassaName)
        {
            try
            {
                // 1. Create new organization and link to user
                var newOrgId = _organizationService.AddOrganizationToNewUser(userId, organizationName);
                if (newOrgId <= 0)
                {
                    Console.WriteLine("Failed to create organization.");
                    return false;
                }

                // 2. Create new kassa for the organization
                var newKassaId = _kassaRepo.CreateKassa(kassaName, newOrgId);
                if (newKassaId <= 0)
                {
                    Console.WriteLine("Failed to create kassa.");
                    return false;
                }

                // 3. Link the new kassa to the user
                var kassaUserRelId = _kassaRepo.InsertKassaToUser(userId, newKassaId,false,true);
                if (kassaUserRelId <= 0)
                {
                    Console.WriteLine("Failed to assign kassa to user.");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddOrganizationAndKassaToExistingUser: {ex.Message}");
                return false;
            }
        }


        public bool UpdatePassword(string otpCode, string content, string pwd,int type)
		{
			if (CheckRecoveryOtpHash(content, otpCode,type))
			{
				UserDAO user = null;
				if(type == 1)
				{
                    user = _userRepository.GetUserByPhone(content);

                }
				else if (type==2)
				{
                    user = _userRepository.GetUserByEmail(content);

                }

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


		public bool CheckOtpHash(string content, string providedOtp,int type)
		{

            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(providedOtp) && (type>0 && type<3))
			{
                var providedOtpHash = sha256(providedOtp);

                var otpData = _userRepository.GetOtpData(content,type);

				if (otpData == providedOtpHash)
				{
					return true;
				}
			}

			return false;
		}

		public bool CheckRecoveryOtpHash(string content, string providedOtp,int type)
		{
            if (!string.IsNullOrEmpty(content) && !string.IsNullOrEmpty(providedOtp) && (type>0 && type<3))
            {
				var providedOtpHash = sha256(providedOtp);

				var otpData = _userRepository.GetRecoveryOtpData(content,type);

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
	  int isActive = 0, string otp = "", int isRegistered = 0)
		{

			return _userRepository.InsertUser(name, surname,
		   father, specialityID, passportSerialNum, fin, phone, email,
			bDate, username, pwd,
			isUser, isDr, isAdmin,
			isActive,otp,isRegistered:isRegistered);

		}


		public long UpdateUser(int userID = 0, string name = "", string surname = "", string father = "",
			int specialityID = 0, string passportSerialNum = "", string fin = "",
			string mobile = "", string email = "", string bDate = "",
			string username = "", int isUser = 0, int isDr = 0, int isActive = 0,
			int isAdmin = 0, string otp = "", string recoveryOtp = "", DateTime? recoveryOtpSendDate = null, string password = "", int isRegistered = 0, string imagePath = "")

        {
			if (!string.IsNullOrWhiteSpace(password))
			{
				password = sha256(password);
			}

			return _userRepository.UpdateUser(userID, name, surname, father,
			 specialityID, passportSerialNum, fin,
			 mobile, email, bDate,
			 username, isUser, isDr, isActive,
			 isAdmin, otp, recoveryOtp, recoveryOtpSendDate, password, isRegistered,imagePath);

		}

        public List<UserDAO> GetRefererList()
        {
            return _userRepository.GetRefererList();
        }

        public UserDAO GetUserById(int id)
		{
			return _userRepository.GetUserByID(id);
		}


		public List<UserDAO> GetUserList(int id = 0)
        {
            return _userRepository.GetUserList(id);
        }

		public async Task<UserDAO> GetOnlyUserById(int id)
		{
			using var con = _unitOfWork.BeginConnection();
			var result=await _userRepository.GetOnlyUserById(id);
			return result;
		}
	}


}




