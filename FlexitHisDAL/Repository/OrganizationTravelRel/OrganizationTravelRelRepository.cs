using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
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
		public async Task<OrganizationTravelDAO?> GetByOrganizationIdAsync(int id)
		{
			string sql = @"SELECT * FROM organization_travel_rel WHERE organizationId=@Id";
			var con = _unitOfWork.GetConnection();
			var result = await con.QuerySingleOrDefaultAsync<OrganizationTravelDAO>(sql, new { Id = id });
			return result;
		}


		public async Task<bool> UpdateAsync(OrganizationTravelDAO dao)
		{
			string sql = $@"
			UPDATE organization_travel_rel SET
            organizationId=@{nameof(OrganizationTravelDAO.organizationId)},
            distance=@{nameof(OrganizationTravelDAO.distance)},
            fee=@{nameof(OrganizationTravelDAO.fee)},
            feeType=@{nameof(OrganizationTravelDAO.feeType)}

			WHERE id=@{nameof(OrganizationTravelDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}
	}
}
