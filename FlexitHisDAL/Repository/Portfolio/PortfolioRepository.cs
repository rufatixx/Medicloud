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
			string sql = @"SELECT * FROM portfolios WHERE organizationId=@OrganizationId";
			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<PortfolioDAO>(sql, new { OrganizationId = organizationId });
			return result.ToList();
		}
	}
}
