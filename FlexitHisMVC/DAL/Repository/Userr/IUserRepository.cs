using Medicloud.Models;

namespace Medicloud.DAL.Repository.Userr
{
	public interface IUserRepository
	{
		Task<User> GetUser(string mobileNumber, string pass);
	}
}
