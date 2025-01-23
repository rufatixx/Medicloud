using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
namespace Medicloud.DAL.Repository.RequestType
{
    public class RequestTypeRepository : IRequestTypeRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestTypeRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<List<RequestTypeDAO>> GetRequestTypesAsync()
        {
            string query = @"SELECT * FROM request_type;";
            var con = _unitOfWork.GetConnection();
            try
            {
                var result = (await con.QueryAsync<RequestTypeDAO>(query)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                //Medicloud.StandardMessages.CallSerilog(ex);
                return new();
            }

        }
    }
}
