using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.OrganizationServiceRel
{
	public class OrganizationServiceRelRepository:IOrganizationServiceRelRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrganizationServiceRelRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(TempRelDAO dao)
		{
			string AddSql = $@"
			INSERT INTO organization_service_rel
            (organizationId,serviceId)
			VALUES (@{nameof(TempRelDAO.FirstModelId)},
            @{nameof(TempRelDAO.SecondModelId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<bool> RemoveAsync(int organizationId, int serviceId)
		{
			string sql = $@"
			UPDATE organization_service_rel SET isActive = 0
			WHERE organizationId=@OrganizationId AND serviceId=@ServiceId";
			var con = _unitOfWork.GetConnection();
			int executed = await con.ExecuteAsync(sql, new { OrganizationId=organizationId, ServiceId =serviceId});
			return executed>0;
		}
	}
}
