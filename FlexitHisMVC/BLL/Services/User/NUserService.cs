using Medicloud.BLL.Models;
using Medicloud.BLL.Utils;
using Medicloud.DAL.DAO;
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
		private readonly HashHelper _hashHelper;
		public NUserService(IUnitOfWork unitOfWork, IUserRepository userRepository)
		{
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
			_hashHelper = new();
		}

        public async Task<UserDAO> GetUserById(int id)
        {
			using var con =  _unitOfWork.BeginConnection();
			return await _userRepository.GetUserById(id);
        }

        public async Task<UserDAO> SignInAsync(string contact,int contactType, string password)
		{
			int userId;
			using var con=_unitOfWork.BeginConnection();
			switch (contactType)
			{
				case 1:userId = await _userRepository.GetUserIdByPhoneNumber(contact);
					break;
				case 2: userId=await _userRepository.GetUserIdByEmail(contact);
					break ;
				default:
					userId=0;
					break;
			}
			if (userId>0)
			{
				var user=await _userRepository.GetUserById(userId);
				if(user!=null && user.isRegistered)
				{
					var hashedPassword = _hashHelper.sha256(password);
					if (user.pwd==hashedPassword)
					{
						return user;
					}
				}
			}
			return null;
		}

		public async Task<int> UpdateUserAsync(UpdateUserDTO userDTO)
		{
			using var con = _unitOfWork.BeginConnection();
			var result=await _userRepository.UpdateUserAsync(userDTO);
			return result;
		}

		//private string sha256(string randomString)
		//{
		//	var crypt = new System.Security.Cryptography.SHA256Managed();
		//	var hash = new System.Text.StringBuilder();
		//	byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
		//	foreach (byte theByte in crypto)
		//	{
		//		hash.Append(theByte.ToString("x2"));
		//	}
		//	return hash.ToString();
		//}

		public async Task<int> AddUser(UserDAO dao)
		{
			using var con =_unitOfWork.BeginConnection();
			var result = await _userRepository.AddUser(dao);
			return result;
		}

		public async Task<UserDAO> GetUserByPhoneNumber(string phoneNumber)
		{
			using var con = _unitOfWork.BeginConnection();
			int userId = await _userRepository.GetUserIdByPhoneNumber(phoneNumber);
			var result = await _userRepository.GetUserById(userId);
			return result;
		}

		public async Task<UserDAO> GetUserByEmail(string email)
		{
			using var con = _unitOfWork.BeginConnection();
			int userId = await _userRepository.GetUserIdByEmail(email);
			var result = await _userRepository.GetUserById(userId);
			return result;
		}
	}
}
