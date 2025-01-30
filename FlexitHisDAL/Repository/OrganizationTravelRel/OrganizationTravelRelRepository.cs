using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.OrganizationTravelRel
{
	public class OrganizationTravelRelRepository:IOrganizationTravelRelRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrganizationTravelRelRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(OrganizationTravelDAO dao)
		{
			string AddSql = $@"
			INSERT INTO organization_travel_rel
            (organizationId,distance,fee,feeType)
			VALUES (@{nameof(OrganizationTravelDAO.organizationId)},
            @{nameof(OrganizationTravelDAO.distance)},
            @{nameof(OrganizationTravelDAO.fee)},
            @{nameof(OrganizationTravelDAO.feeType)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
	}
}
