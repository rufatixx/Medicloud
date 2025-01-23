

using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Role
{
	public interface IRoleRepository
	{
		Task<List<RoleDAO>> GetUserRoles(int organizationId, int userId);
		Task<int> AddUserRole(int organizationId, int userId,int roleId);
		Task RemoveUserRole(int organizationId, int userId,int roleId);
	}
}
