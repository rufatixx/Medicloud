using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.Staff
{
    public class StaffRepository:IStaffRepository
    {
		private readonly IUnitOfWork _unitOfWork;

		public StaffRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(StaffDAO dao)
		{
			string AddSql = $@"
			INSERT INTO users
            (name,phoneNumber,email,organizationId)
			VALUES (@{nameof(StaffDAO.name)},
            @{nameof(StaffDAO.phoneNumber)},
            @{nameof(StaffDAO.email)},
            @{nameof(StaffDAO.organizationId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
	}
}
