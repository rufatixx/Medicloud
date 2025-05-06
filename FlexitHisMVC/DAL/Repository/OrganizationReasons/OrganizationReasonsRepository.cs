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

		public async Task<List<OrganizationReasonDAO>> GetByOrganizationId(int organizationId,bool isActive=false)
		{
			string activeCondition = "";
			if (isActive)
			{
				activeCondition = " AND isActive=1";
			}
            string sql = @$"SELECT * FROM organization_reasons WHERE organizationId=@OrganizationId {activeCondition}";
            var con = _unitOfWork.BeginConnection();
            var result = await con.QueryAsync<OrganizationReasonDAO>(sql, new { OrganizationId =organizationId});
            return result.ToList();
        }

		public async Task<bool> UpdateAsync(OrganizationReasonDAO dao)
		{
			string sql = @"UPDATE organization_reasons SET 
					name=@Name,
					isActive=@IsActive
					WHERE id=@Id
				";

			var con = _unitOfWork.BeginConnection();
			int result = await con.ExecuteAsync(sql, new
			{
				Name = dao.name,
				IsActive = dao.isActive?1:0,
				Id = dao.id
			});
			return result >0;
		}
	}
}
