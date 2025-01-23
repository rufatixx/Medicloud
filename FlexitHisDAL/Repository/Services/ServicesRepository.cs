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

        public async Task<List<ServiceDAO>> GetServicesByOrganizationAsync(int organizationID)
        {
            string query = @"SELECT * FROM services where organizationID=@organizationID and isActive=1 order by id desc;";
            var con=_unitOfWork.GetConnection();
            var result=(await con.QueryAsync<ServiceDAO>(query, new { organizationID })).ToList();
            return result;

        }
    }
}
