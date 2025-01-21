using Medicloud.BLL.Models;
using Medicloud.DAL.DAO;
using Medicloud.Models.DTO;

namespace Medicloud.BLL.Services.User
{
	public interface INUserService
	{
		Task<UserDAO> SignInAsync(string contact,int contactType,string password);
		Task<UserDAO> GetUserById(int id);
		Task<UserDAO> GetUserByPhoneNumber(string phoneNumber);
		Task<UserDAO> GetUserByEmail(string email);
		Task<int> UpdateUserAsync(UpdateUserDTO userDTO);
		Task<int> AddUser(UserDAO dao);
	}
}
