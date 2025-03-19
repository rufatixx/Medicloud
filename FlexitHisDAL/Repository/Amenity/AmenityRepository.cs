using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.Amenity
{
	public class AmenityRepository : IAmenityRepository

	{
		private readonly IUnitOfWork _unitOfWork;

		public AmenityRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<TempDAO>> GetAll()
		{
			var con = _unitOfWork.GetConnection();
			var data = await con.QueryAsync<TempDAO>("SELECT * FROM amenities");
			return data;
		}

		public async Task<int> AddOrganizationAmenityAsync(int organizationId, int amenityId)
		{
			string AddSql = $@"
			INSERT INTO organization_amenities
            (organizationId,amenityId)
			VALUES (@OrganizationId,@AmenityId);

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, new { OrganizationId = organizationId, AmenityId = amenityId });
			return newId;
		}

		public async Task<IEnumerable<int>> GetOrganizationAmenityIds(int organizationId)
		{
			string sql = @"SELECT a.id  
					 FROM amenities a 
					 LEFT JOIN organization_amenities oa ON a.id = oa.amenityId
					 WHERE oa.organizationId=@OrganizationId AND oa.isActive=1";
			var con = _unitOfWork.GetConnection();
			var data = await con.QueryAsync<int>(sql, new { OrganizationId = organizationId });
			return data;
		}

		public async Task RemoveOrganizationAmenities(int organizationId, IEnumerable<int> removedIds)
		{
			var con= _unitOfWork.GetConnection();
			await con.ExecuteAsync("UPDATE organization_amenities SET isActive= 0 WHERE organizationId=@OrganizationId AND amenityId IN @AmenityIds", new
			{
				OrganizationId = organizationId,
				AmenityIds = removedIds,
			});
		}
	}
}
