using Dapper;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.Models;

namespace Medicloud.DAL.Repository.Concrete
{
    public class RequestTypeRepository : IRequestTypeRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public RequestTypeRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork=unitOfWork;
        }

        public async Task<List<RequestType>> GetRequestTypesAsync()
        {
            string query = @"SELECT * FROM request_type;";
            var con = _unitOfWork.GetConnection();
            try
            {
                var result = (await con.QueryAsync<RequestType>(query)).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                return new();
            }

        }
    }
}
