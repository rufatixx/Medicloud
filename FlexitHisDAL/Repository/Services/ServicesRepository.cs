using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;


namespace Medicloud.DAL.Repository.Services
{
    public class ServicesRepository : IServicesRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServicesRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationId)
        {
			//string query = @"SELECT * FROM services where organizationID=@organizationID and isActive=1 order by id desc;";
			string query = @"SELECT * 
							FROM services s
							LEFT JOIN organization_service_rel osr ON S.id = osr.Id
							where osr.organizationId=@OrganizationId and osr.isActive=1 order by osr.id desc;";

			var con =_unitOfWork.GetConnection();
            var result=(await con.QueryAsync<ServiceDAO>(query, new { OrganizationId=organizationId })).ToList();
            return result;

        }

		public async Task<int> AddServiceAsync(ServiceDAO dao)
		{
			string AddSql = $@"
			INSERT INTO services
            (name,price,time,isMobile,typeId,isPriceStart)
			VALUES (@{nameof(ServiceDAO.name)},
            @{nameof(ServiceDAO.price)},
            @{nameof(ServiceDAO.time)},
            @{nameof(ServiceDAO.isMobile)},
            @{nameof(ServiceDAO.typeId)},
            @{nameof(ServiceDAO.isPriceStart)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<bool> UpdateServiceAsync(ServiceDAO dao)
		{
			string sql = $@"
			UPDATE services SET
            name=@{nameof(ServiceDAO.name)},
            price=@{nameof(ServiceDAO.price)},
            time=@{nameof(ServiceDAO.time)},
            isMobile=@{nameof(ServiceDAO.isMobile)},
            isPriceStart=@{nameof(ServiceDAO.isPriceStart)},
            typeId=@{nameof(ServiceDAO.typeId)}

			WHERE id=@{nameof(ServiceDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}

		public async Task<ServiceDAO> GetServiceByIdAsync(int serviceId)
		{
			string query = @"SELECT * 
							FROM services
							WHERE id=@ServiceId";

			var con = _unitOfWork.GetConnection();
			var result = await con.QuerySingleOrDefaultAsync<ServiceDAO>(query, new { ServiceId = serviceId });
			return result;
		}
	}
}
