using Medicloud.BLL.Models;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.Userr
{
	public interface IUserRepository
	{
		Task<User> GetUser(string mobileNumber, string pass);
		Task<int> UpdateUserAsync(UpdateUserDTO userDTO);
		Task<int> GetUserIdByPhoneNumber(string mobileNumber);
	}
}
