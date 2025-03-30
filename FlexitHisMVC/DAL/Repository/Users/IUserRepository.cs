using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.Users
{
    public interface IUserRepository
    {
        bool UserExists(string mobile);
        List<UserDAO> GetUserList(long organizationID = 0);
        List<UserDAO> GetRefererList();
        UserDAO GetUser(string content, string pass, int type);
        Task<UserDAO?> GetUser(string mobileNumber, string pass); 
        UserDAO GetUserByID(int id);
        UserDAO? GetUserByPhone(string mobileNumber);
        UserDAO? GetUserByEmail(string email);
        long InsertUser(string name = "", string surname = "", string father = "", int specialityID = 0,
            string passportSerialNum = "", string fin = "", string phone = "", string email = "", string bDate = "",
            string username = "", string pwd = "", int isUser = 0, int isDr = 0, int isAdmin = 0, int isActive = 0,
            string otp = "", string imagePath = "", int isRegistered = 0);
        int UpdateUser(int userID = 0, string name = "", string surname = "", string father = "",
            int specialityID = 0, string passportSerialNum = "", string fin = "",
            string mobile = "", string email = "", string bDate = "", string username = "",
            int isUser = 0, int isDr = 0, int isActive = 0, int isAdmin = 0, string otp = "",
            string recoveryOtp = "", DateTime? recoveryOtpSendDate = null,
            string password = "", int isRegistered = 0, string imagePath = "");
        int UpdateUserPwd(int userID, string pwd);
        int UpdateUserExpireDate(int userId, DateTime expireDate);
        string GetOtpData(string content, int type);
        string GetRecoveryOtpData(string content, int type);
    }
}
