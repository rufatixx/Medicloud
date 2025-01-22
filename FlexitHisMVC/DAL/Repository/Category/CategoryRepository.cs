using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.Abstract;

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
	}
}
