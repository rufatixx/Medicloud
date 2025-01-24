
using Medicloud.BLL.Utils;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.OTP;

namespace Medicloud.BLL.Services.OTP
{
	public class OtpService:IOtpService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOTPRepository _otpRepository;
		private readonly HashHelper _hashHelper;

		public OtpService(IUnitOfWork unitOfWork, IOTPRepository otpRepository)
		{
			_unitOfWork = unitOfWork;
			_otpRepository = otpRepository;
			_hashHelper = new();
		}

		public async Task<int> CreateOtp(int type,int userId,string otpCode)
		{
			var hashOtp = _hashHelper.HashOtp(otpCode);
			var otpModel = new OTPCodeDAO()
			{
				created_date = DateTime.Now,
				expiration_date = DateTime.Now.AddMinutes(5),
				otp_code = hashOtp,
				is_used = false,
				otp_type =type,
				user_id = userId
			};

			using var con = _unitOfWork.BeginConnection();
			var result = await _otpRepository.AddOTP(otpModel);

			return result;
		}

		public async Task<bool> CheckOtp(string userOtpCode, int userId)
		{
			await using var con = _unitOfWork.BeginConnection();
			var userOtp = await _otpRepository.GetActiveOtpByUserId(userId);
			if (userOtpCode == userOtp.otp_code)
			{
				await _otpRepository.UpdateOtpStatus(userOtp.id);
				return true;
			}

			return false;
		}

		public async Task<OTPCodeDAO> GetActiveOtpByUserId(int userId)
		{
			await using var con = _unitOfWork.BeginConnection();
			var otp = await _otpRepository.GetActiveOtpByUserId(userId);
			return otp;
		}

		public async Task<OTPCodeDAO> GetOtpById(int otpId)
		{
			await using var con = _unitOfWork.BeginConnection();
			var otp = await _otpRepository.GetOtpById(otpId);
			return otp;
		}
	}
}
