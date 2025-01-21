using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.OTP
{
	public class OTPRepository:IOTPRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public OTPRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddOTP(OTPCodeDAO dao)
		{
			const string sql =
				$@"INSERT otp_codes (user_id, otp_code, otp_type, created_date, expiration_date, is_used)
						    VALUES (
				@{nameof(OTPCodeDAO.user_id)},
				@{nameof(OTPCodeDAO.otp_code)},
				@{nameof(OTPCodeDAO.otp_type)},
				@{nameof(OTPCodeDAO.created_date)},
				@{nameof(OTPCodeDAO.expiration_date)},
				@{nameof(OTPCodeDAO.is_used)});

				SELECT LAST_INSERT_ID();";

			try
			{
				var con = _unitOfWork.GetConnection();
				var otpId = await con.QuerySingleOrDefaultAsync<int>(sql, dao);
				return otpId;
			}
			catch (Exception)
			{
				Console.WriteLine("Add otp code error");
				throw;
			}
		}

		public async Task<OTPCodeDAO> GetActiveOtpByUserId(int userId)
		{
			const string sql = @"
SELECT * FROM otp_codes
WHERE is_used=0
AND expiration_date > @CurrentTime
AND user_id = @userId";

			try
			{
				var con = _unitOfWork.GetConnection();
				var otpCode = await con.QuerySingleOrDefaultAsync<OTPCodeDAO>(sql, new { userId, CurrentTime = DateTime.Now });

				return otpCode;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<OTPCodeDAO> GetOtpById(int otpId)
		{
			const string sql = @"
SELECT * FROM otp_codes
WHERE id=@otpId";

			try
			{
				var con = _unitOfWork.GetConnection();
				var otp = await con.QuerySingleOrDefaultAsync<OTPCodeDAO>(sql, new { otpId });

				return otp;
			}
			catch (Exception e)
			{
				throw;
			}
		}

		public async Task UpdateOtpStatus(int otpId)
		{
			const string sql = $@"UPDATE otp_codes SET is_used=1 WHERE id=@id";

			try
			{
				var con = _unitOfWork.GetConnection();
				await con.ExecuteAsync(sql, new { id = otpId });
			}
			catch (Exception e)
			{
				throw;
			}
		}

	}
}
