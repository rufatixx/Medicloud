using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.Portfolio
{
	public class PortfolioRepository : IPortfolioRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public PortfolioRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(PortfolioDAO dao)
		{
			string AddSql = $@"
			INSERT INTO portfolios
            (description,fileId,organizationId)
			VALUES (@{nameof(PortfolioDAO.description)},
            @{nameof(PortfolioDAO.fileId)},
            @{nameof(PortfolioDAO.organizationId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);

			if (!string.IsNullOrEmpty(dao.categoryIds))
			{
				var categories = dao.categoryIds.Split(',');
				foreach (var categoryId in categories)
				{
					await con.QuerySingleOrDefaultAsync<int>(
					@"INSERT INTO portfolio_categories (portfolioId,categoryId)
						VALUES(@PortfolioId,@CategoryId);
						SELECT LAST_INSERT_ID();",
					new { PortfolioId = newId, CategoryId = int.Parse(categoryId) });
				}
			}
			return newId;
		}

		public async Task<List<PortfolioDAO>> GetByOrganizationIdAsync(int organizationId)
		{
			string sql = @"SELECT * FROM portfolios WHERE organizationId=@OrganizationId AND isActive=1";
			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<PortfolioDAO>(sql, new { OrganizationId = organizationId });
			return result.ToList();
		}
		public async Task<PortfolioDAO> GetByIdAsync(int id)
		{
			Console.WriteLine(id);

			string sql = @"SELECT p.*,
						GROUP_CONCAT(pc.categoryId) AS categoryIds
						FROM portfolios p
						LEFT JOIN portfolio_categories pc ON p.id = pc.portfolioId AND pc.isActive = 1
						WHERE p.id=@Id AND p.isActive=1";
			var con = _unitOfWork.GetConnection();
			var result = await con.QuerySingleOrDefaultAsync<PortfolioDAO>(sql, new { Id = id });
			return result;
		}

		public async Task UpdateAsync(PortfolioDAO dao)
		{
			string sql = $@"UPDATE portfolios SET description = @Description WHERE id=@Id";
			var con = _unitOfWork.GetConnection();
			await con.ExecuteAsync(sql, new { Description =dao.description,Id=dao.id});

		}

		public async Task RemoveCategoriesFromPortfolio(int portfolioId, List<int> categoryIds)
		{
			string sql = $@"UPDATE portfolio_categories SET isActive=0 WHERE portfolioId=@PortfolioId AND categoryId IN @CategoryIds";
			var con = _unitOfWork.GetConnection();
			await con.ExecuteAsync(sql, new { PortfolioId = portfolioId, CategoryIds=categoryIds });
		}

		public async Task<int> AddCategoryToPortfolio(int portfolioId,int categoryId)
		{
			var con = _unitOfWork.GetConnection();
			return await con.QuerySingleOrDefaultAsync<int>(
			@"INSERT INTO portfolio_categories (portfolioId,categoryId)
						VALUES(@PortfolioId,@CategoryId);
						SELECT LAST_INSERT_ID();",
			new { PortfolioId = portfolioId, CategoryId = categoryId });
		}

		public async Task DeleteAsync(int id)
		{
			string sql = $@"UPDATE portfolios SET isActive=0 WHERE id=@PortfolioId";
			var con = _unitOfWork.GetConnection();
			await con.ExecuteAsync(sql, new { PortfolioId = id });
		}
	}
}
