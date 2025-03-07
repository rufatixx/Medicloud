
using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.Organizationn
{
	public class OrganizationCategoryRelRepository : IOrganizationCategoryRelRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrganizationCategoryRelRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(int organizationId, int categoryId)
		{
			string addSql = @"INSERT INTO organization_category_rel(organization_id,category_id) 
							VALUES(@OrganizationId,@CategoryId)";
			var con = _unitOfWork.BeginConnection();
			var result = await con.QuerySingleOrDefaultAsync<int>(addSql, new { OrganizationId = organizationId, CategoryId = categoryId });
			return result;
		}

		public async Task<List<TempRelDAO>> GetByOrganizationId(int organizationId)
		{
			string query = @"SELECT ocr.id,
							ocr.organization_id as FirstModelId,
							ocr.category_id as SecondModelId
							FROM organization_category_rel ocr
							WHERE ocr.organization_id = @OrganizationId AND ocr.isActive=1";

			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<TempRelDAO>(query, new { OrganizationId = organizationId });
			return result.ToList();
		}

		public async Task RemoveAsync(int id)
		{
			string query = $"UPDATE organization_category_rel SET isActive = 0 WHERE id=@Id";
			var con = _unitOfWork.GetConnection();
			await con.ExecuteAsync(query, new {Id=id});
		}
	}
}
