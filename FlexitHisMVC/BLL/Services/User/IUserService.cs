using Medicloud.Models.DTO;

namespace Medicloud.BLL.Services.User
{
	public interface IUserService
	{
		Task<UserDTO> SignInAsync(string mobile,string password);
	}
}
