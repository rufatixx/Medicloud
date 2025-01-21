using Medicloud.BLL.Models;
using Medicloud.DAL.DAO;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.Userr
{
	public interface IUserRepository
	{
		Task<User> GetUser(string mobileNumber, string pass);
		Task<UserDAO> GetUserById(int id);
		Task<int> AddUser(UserDAO dao);
		Task<int> UpdateUserAsync(UpdateUserDTO userDTO);
		Task<int> GetUserIdByPhoneNumber(string mobileNumber);
		Task<int> GetUserIdByEmail(string email);

    }
}
