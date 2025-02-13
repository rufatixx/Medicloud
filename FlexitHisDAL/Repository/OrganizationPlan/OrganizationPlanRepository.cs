using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.OrganizationPlan
{
	public class OrganizationPlanRepository:IOrganizationPlanRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrganizationPlanRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(OrganizationPlanDAO dao)
		{
			string AddSql = $@"
			INSERT INTO organization_plans
            (createDate,expireDate,planId,organizationId)
			VALUES (@{nameof(OrganizationPlanDAO.createDate)},
            @{nameof(OrganizationPlanDAO.expireDate)},
            @{nameof(OrganizationPlanDAO.planId)},
            @{nameof(OrganizationPlanDAO.organizationId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<bool> UpdateAsync(OrganizationPlanDAO dao)
		{
			string sql = $@"
			UPDATE organization_plans SET
            createDate=@{nameof(OrganizationPlanDAO.createDate)},
            expireDate=@{nameof(OrganizationPlanDAO.expireDate)}

			WHERE id= @{nameof(OrganizationPlanDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}

		public async Task DeleteAsync(int id)
		{
			string sql = $@"
			UPDATE organization_plans SET isActive=0 WHERE id= @Id";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, new {Id=id});
		}

		public async Task<List<OrganizationPlanDAO>> GetPlansByOrganizationId(int organizationId)
		{
			string query = @"SELECT * FROM organization_plans WHERE organizationId=@OrganizationId AND isActive=1 ";

			var con = _unitOfWork.GetConnection();
			var result = (await con.QueryAsync<OrganizationPlanDAO>(query, new { OrganizationId = organizationId })).ToList();
			return result;
		}
	}
}
