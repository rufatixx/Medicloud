using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Userr;
using Medicloud.Models.DTO;

namespace Medicloud.BLL.Services.User
{
	public class UserService:IUserService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IUserRepository _userRepository;
		public Task<UserDTO> SignInAsync(string mobile, string password)
		{
			throw new NotImplementedException();
		}
	}
}
