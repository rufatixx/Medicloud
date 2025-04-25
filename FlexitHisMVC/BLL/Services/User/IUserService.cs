using Medicloud.BLL.DTO;
using Medicloud.Models.DTO;
using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Services
{
    public interface IUserService
    {
        Task<UserDTO> SignIn(string content, string pass, int type);
        List<UserDAO> GetRefererList();
        Task<OtpResult> SendOtpForUserRegistration(string content, int type);
        Task<OtpResult> SendRecoveryOtpForUser(string content, int type);
        Task<(int userId, long organizationId)> AddUser(string phone, string email, string name, string surname, string father, int specialityID, string fin, string bDate, string pwd, string organizationName, int planID, string imagePath);
        bool AddOrganizationAndKassaToExistingUser(int userId, string organizationName, string kassaName);
        bool UpdatePassword(string otpCode, string content, string pwd, int type);
        bool CheckOtpHash(string content, string providedOtp, int type);
        bool CheckRecoveryOtpHash(string content, string providedOtp, int type);
        long InsertUser(string name = "", string surname = "",
            string father = "", int specialityID = 0, string passportSerialNum = "",
            string fin = "", string phone = "", string email = "",
            string bDate = "", string username = "", string pwd = "",
            int isUser = 0, int isDr = 0, int isAdmin = 0,
            int isActive = 0, string otp = "",int isRegistered = 0);
        long UpdateUser(int userID = 0, string name = "", string surname = "", string father = "",
            int specialityID = 0, string passportSerialNum = "", string fin = "",
            string mobile = "", string email = "", string bDate = "",
            string username = "", int isUser = 0, int isDr = 0, int isActive = 0,
            int isAdmin = 0, string otp = "", string recoveryOtp = "", DateTime? recoveryOtpSendDate = null, string password = "",  int isRegistered = 0, string imagePath = "");
        UserDAO GetUserById(int id);
        List<UserDAO> GetUserList(int id = 0);
        bool SaveSession(HttpContext context, string key, string value);
        bool ResetUserKassaSession(HttpContext context, string organizationID, string userID);
        bool SaveCookie(HttpContext context, string key, string value);
        bool ResetUserKassaCookies(HttpContext context, string organizationID, string userID);
		Task<UserDAO> GetOnlyUserById(int id);

	}
}
