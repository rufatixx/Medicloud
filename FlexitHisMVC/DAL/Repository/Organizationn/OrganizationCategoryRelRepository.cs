
using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;

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
	}
}
