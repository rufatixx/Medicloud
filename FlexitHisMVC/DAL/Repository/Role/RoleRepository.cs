using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;
using Medicloud.Repository;
using MySqlX.XDevAPI.Common;

namespace Medicloud.DAL.Repository.Role
{
	public class RoleRepository : IRoleRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public RoleRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<RoleDTO>> GetUserRoles(int organizationId, int userId)
		{
			using var con = _unitOfWork.BeginConnection();
			string sqlQuery = @$"SELECT 
					r.id AS id,
					r.name AS name
                    FROM medicloud.user_organization_rel uhr
					LEFT JOIN medicloud.roles r ON r.id = uhr.role_id 
					WHERE uhr.organizationID=@OrganizationId AND uhr.userID=@UserId AND uhr.is_active=1 ";

			var result = await con.QueryAsync<RoleDTO>(sqlQuery, new { OrganizationId = organizationId, UserId = userId });
			return result.ToList();
		}

		public async Task<int> AddUserRole(int organizationId, int userId, int roleId)
		{
			using var con = _unitOfWork.BeginConnection();
			string getQuery = @$"SELECT id FROM user_organization_rel WHERE  organizationID=@OrganizationId AND userID = @UserId AND role_id = @RoleID";
			int existId = await con.QueryFirstOrDefaultAsync<int>(getQuery, new { OrganizationId = organizationId, UserId = userId, RoleID = roleId });
			if (existId == 0)
			{
				string sql = @"Insert INTO user_organization_rel (organizationID,userID,role_id) VALUES(@OrganizationId,@UserId,@RoleID);
					SELECT LAST_INSERT_ID()";
				var result = await con.QuerySingleOrDefaultAsync<int>(sql, new { OrganizationId = organizationId, UserId = userId, RoleID = roleId });
				return result;

			}
			else
			{
				string sql = @"UPDATE user_organization_rel SET is_active = 1 WHERE id=@Id";
				await con.ExecuteAsync(sql, new { Id = existId });
				return existId;

			}

		}



		public async Task RemoveUserRole(int organizationId, int userId, int roleId)
		{
			string sql = @"UPDATE user_organization_rel SET is_active = 0 WHERE organizationID = @OrganizationId AND userID = @UserId AND role_id = @RoleID;";
			using var con = _unitOfWork.BeginConnection();
			await con.ExecuteAsync(sql, new { OrganizationId = organizationId, UserId = userId, RoleID = roleId });
		}
	}
}
