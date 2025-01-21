using Medicloud.DAL.DAO;

namespace Medicloud.BLL.Services.OTP
{
	public interface IOtpService
	{
		Task<int> CreateOtp(int type,int userId,string otpCode);
		Task<bool> CheckOtp(string userOtpCode, int userId);
		Task<OTPCodeDAO> GetActiveOtpByUserId(int userId);
		Task<OTPCodeDAO> GetOtpById(int otpId);
	}
}
