using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.Services
{
	public class ServiceTypeRepository:IServiceTypeRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public ServiceTypeRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<ServiceTypeDAO>> GetAllTypes()
		{
			string sql = @"SELECT * FROM service_type";
			var con= _unitOfWork.GetConnection();
			var result=await con.QueryAsync<ServiceTypeDAO>(sql);
			return result.ToList();
		}

		public async Task<List<TempDAO>> GetTypeCategories()
		{
			string sql = @"SELECT * FROM service_type_categories";
			var con = _unitOfWork.GetConnection();
			var result = await con.QueryAsync<TempDAO>(sql);
			return result.ToList();
		}
	}
}
