using Medicloud.BLL.Models;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Userr;
using Medicloud.Models.DTO;
using System.Text;

namespace Medicloud.BLL.Services.User
{
	public class NUserService:INUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IUserRepository _userRepository;

		public NUserService(IUnitOfWork unitOfWork, IUserRepository userRepository)
		{
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
		}

		public Task<UserDTO> SignInAsync(string mobile, string password)
		{
			throw new NotImplementedException();
		}

		public async Task<int> UpdateUserAsync(string phoneNumber ,UpdateUserDTO userDTO)
		{
			using var con = _unitOfWork.BeginConnection();
			var userId= await _userRepository.GetUserIdByPhoneNumber(phoneNumber);
			userDTO.ID=userId;
			userDTO.pwd = sha256(userDTO.pwd);
			var result=await _userRepository.UpdateUserAsync(userDTO);
			return result;
		}


		private string sha256(string randomString)
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
	}
}
