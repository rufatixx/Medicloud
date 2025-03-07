using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;

namespace Medicloud.DAL.Repository.OrganizationPhoto
{
	public class OrganizationPhotoRepository:IOrganizationPhotoRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public OrganizationPhotoRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(int organizationId, int fileId)
		{
			string addSql = @"INSERT INTO organization_photos(organizationId,fileId) 
							VALUES(@OrganizationId,@FileId)";
			var con = _unitOfWork.BeginConnection();
			var result = await con.QuerySingleOrDefaultAsync<int>(addSql, new { OrganizationId = organizationId, FileId = fileId });
			return result;
		}

		public async Task<List<FileDAO>> GetByOrganizationId(int organizationId)
		{
			string query = @"SELECT f.*
							FROM files f
							LEFT JOIN organization_photos op ON op.fileId=f.id 
							WHERE op.organizationId=@OrganizationId AND f.isActive=1";

			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<FileDAO>(query, new { OrganizationId = organizationId });
			return result.ToList();
		}
	}
}
