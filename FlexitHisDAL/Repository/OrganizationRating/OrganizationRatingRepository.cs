using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.OrganizationRating
{
	public class OrganizationRatingRepository:IOrganizationRatingRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrganizationRatingRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(OrganizationRatingDAO organizationRatingDAO)
		{
			string AddSql = $@"
			INSERT INTO organizations
            (organizationId,userId,rating,cDate,address,latitude,longitude,logoId,cDate,ownerId)
			VALUES (@{nameof(OrganizationRatingDAO.organizationId)},
            @{nameof(OrganizationRatingDAO.userId)},
            @{nameof(OrganizationRatingDAO.rating)},
            @{nameof(OrganizationRatingDAO.cDate)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, organizationRatingDAO);
			return newId;
		}

		public Task<int> RemoveAsync(int organizationRatingId)
		{
			throw new NotImplementedException();
		}
	}
}
