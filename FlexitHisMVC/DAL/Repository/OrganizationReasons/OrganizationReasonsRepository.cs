using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.OrganizationReasons
{
	public class OrganizationReasonsRepository:IOrganizationReasonsRepository
	{
		private readonly IUnitOfWork _unitOfWork;

        public OrganizationReasonsRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<int> AddAsync(OrganizationReasonDAO dao)
		{
            string AddSql = $@"
			INSERT INTO organization_reasons
            (name,organizationID)
			VALUES (@{nameof(OrganizationReasonDAO.name)},
            @{nameof(OrganizationReasonDAO.organizationId)});

			SELECT LAST_INSERT_ID();";
            var con = _unitOfWork.BeginConnection();
            var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
            return newId;
        }

		public async Task<List<OrganizationReasonDAO>> GetByOrganizationId(int organizationId)
		{
            string sql = @"SELECT * FROM organization_reasons WHERE organizationId=@OrganizationId AND isActive=1";
            var con = _unitOfWork.BeginConnection();
            var result = await con.QueryAsync<OrganizationReasonDAO>(sql, new { OrganizationId =organizationId});
            return result.ToList();
        }
	}
}
