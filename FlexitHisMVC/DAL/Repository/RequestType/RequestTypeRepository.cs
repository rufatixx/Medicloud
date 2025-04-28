using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.RequestType
{
	public class RequestTypeRepository:IRequestTypeRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public RequestTypeRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<RequestTypeDAO>> GetRequestTypes(int organizationId = 0)
		{
			Console.WriteLine($"repo {organizationId}");
			string sql = @"SELECT * FROM request_type WHERE organization_id=@OrganizationId";
			using var con= _unitOfWork.BeginConnection();
			var result=await con.QueryAsync<RequestTypeDAO>(sql, new { OrganizationId =organizationId});	
			return result.ToList();
		}
	}
}
