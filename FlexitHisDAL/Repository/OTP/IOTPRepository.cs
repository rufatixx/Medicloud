
using Medicloud.DAL.DAO;
namespace Medicloud.DAL.Repository.OTP
{
	public interface IOTPRepository
	{
		Task<int> AddOTP(OTPCodeDAO dao);
		Task<OTPCodeDAO> GetActiveOtpByUserId(int userId);
		Task<OTPCodeDAO> GetOtpById(int otpId);
		Task UpdateOtpStatus(int otpId);
	}
}
