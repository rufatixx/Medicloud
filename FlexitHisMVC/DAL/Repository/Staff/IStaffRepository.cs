using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.Staff
{
    public interface IStaffRepository
    {
		Task<int> AddAsync(StaffDAO dao);
    }
}
