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

		public async Task<StaffDAO> GetOwnerStaffByOrganizationId(int organizationId)
		{
			string sql = @"SELECT * FROM staff WHERE permissionLevelId = 1 AND organizationId = @OrgId";
			var con = _unitOfWork.GetConnection();
			var result = await con.QuerySingleOrDefaultAsync<StaffDAO>(sql,new { OrgId = organizationId});
			return result;
		}

		public async Task<bool> UpdateStaffAsync(StaffDAO dao)
		{
			string sql = $@"
			UPDATE staff SET
            name=@{nameof(StaffDAO.name)},
            phoneNumber=@{nameof(StaffDAO.phoneNumber)},
            email=@{nameof(StaffDAO.email)},
            permissionLevelId=@{nameof(StaffDAO.permissionLevelId)}

			WHERE id= @{nameof(StaffDAO.id)}";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, dao);
			return result > 0;
		}
	}
}
