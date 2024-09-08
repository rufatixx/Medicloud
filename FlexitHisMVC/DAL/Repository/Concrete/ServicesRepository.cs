using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.Concrete
{
    public class ServicesRepository : IServicesRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServicesRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<List<ServiceObj>> GetServicesByOrganizationAsync(int organizationID)
        {
            string query = @"SELECT * FROM services where organizationID=@organizationID and isActive=1 order by id desc;";
            var con=_unitOfWork.GetConnection();
            var result=(await con.QueryAsync<ServiceObj>(query, new { organizationID })).ToList();
            return result;

        }
    }
}
