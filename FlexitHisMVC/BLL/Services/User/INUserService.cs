using Medicloud.BLL.Models;
using Medicloud.Models.DTO;

namespace Medicloud.BLL.Services.User
{
	public interface INUserService
	{
		Task<UserDTO> SignInAsync(string mobile,string password);
		Task<int> UpdateUserAsync(string phoneNumber, UpdateUserDTO userDTO);
	}
}
