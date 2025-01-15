using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;
using Medicloud.Repository;

namespace Medicloud.DAL.Repository.Role
{
	public class RoleRepository:IRoleRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public RoleRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<RoleDTO>> GetUserRoles(int organizationId, int userId)
		{
			using var con= _unitOfWork.BeginConnection();
			string sqlQuery = @$"SELECT 
					r.id AS id,
					r.name AS name
                    FROM medicloud.user_organization_rel uhr
					LEFT JOIN medicloud.roles r ON r.id = uhr.role_id
					WHERE uhr.organizationID=@OrganizationId AND uhr.userID=@UserId";

			var result = await con.QueryAsync<RoleDTO>(sqlQuery, new { OrganizationId = organizationId, UserId = userId });
			return result.ToList();
		}

		public async Task<int> AddUserRole(int organizationId, int userId, int roleId)
		{

			//string sqlQuery = @$"INSERT INTO  medicloud.user_organization_rel (organizationID,userID) ";
			string sql = @"Insert INTO user_organization_rel (organizationID,userID,role_id) VALUES(@OrganizationId,@UserId,@RoleID);
					SELECT LAST_INSERT_ID()";
			using var con = _unitOfWork.BeginConnection();
			var result = await con.QuerySingleOrDefaultAsync<int>(sql, new { OrganizationId = organizationId, UserId = userId, RoleID=roleId });
			return result;
		}
	}
}
