using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.Role
{
	public interface IRoleRepository
	{
		Task<List<RoleDTO>> GetUserRoles(int organizationId, int userId);
		Task<int> AddUserRole(int organizationId, int userId,int roleId);
	}
}
