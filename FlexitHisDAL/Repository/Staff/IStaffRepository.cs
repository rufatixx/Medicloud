using Medicloud.DAL.DAO;

namespace Medicloud.DAL.Repository.Staff
{
    public interface IStaffRepository
    {
		Task<int> AddAsync(StaffDAO dao);
    }
}
