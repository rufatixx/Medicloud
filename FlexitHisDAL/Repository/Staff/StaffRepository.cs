using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;


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
			INSERT INTO staff
            (name,phoneNumber,email,organizationId,permissionLevelId,userId)
			VALUES (@{nameof(StaffDAO.name)},
            @{nameof(StaffDAO.phoneNumber)},
            @{nameof(StaffDAO.email)},
            @{nameof(StaffDAO.organizationId)},
            @{nameof(StaffDAO.permissionLevelId)},
            @{nameof(StaffDAO.userId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
	}
}
