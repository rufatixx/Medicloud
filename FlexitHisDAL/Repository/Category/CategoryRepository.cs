using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.Category
{
    public class CategoryRepository:ICategoryRepository
    {
		private readonly IUnitOfWork _unitOfWork;

		public CategoryRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<CategoryDAO>> GetAll()
		{
			string query = @"SELECT * FROM categories";
			var con= _unitOfWork.GetConnection();
			var result=await con.QueryAsync<CategoryDAO>(query);
			return result.ToList();
		}

		public async Task<List<CategoryDAO>> GetByOrganizationId(int organizationId)
		{
			string query = @"SELECT c.* 
							FROM categories c
							LEFT JOIN organization_category_rel ocr ON c.id = ocr.category_id
							WHERE ocr.organization_id = @OrganizationId AND ocr.isActive=1";
							
			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<CategoryDAO>(query,new { OrganizationId=organizationId});
			return result.ToList();
		}
	}
}
