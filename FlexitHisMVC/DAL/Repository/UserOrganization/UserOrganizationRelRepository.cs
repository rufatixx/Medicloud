using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.UserOrganization
{
	public class UserOrganizationRelRepository:IUserOrganizationRelRepository
	{
		private readonly IUnitOfWork _unitOfWork;

		public UserOrganizationRelRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<List<UserDAO>> GetDoctorsByOrganization(int organizationID)
		{
//			var sql = $@"SELECT a.*, b.name,b.isDr,b.specialityID, b.surname, c.name as speciality
//FROM user_organization_rel a
//INNER JOIN users b ON a.userID = b.id
//WHERE a.organizationID = @organizationID AND a.role_id =4 AND a.is_active=1 group by userID;";


			string query = $@"
                SELECT u.id,u.name,u.surname, s.name AS specialityName
                FROM users u
                LEFT JOIN user_organization_rel uhr ON u.id = uhr.userID
                LEFT JOIN speciality s ON u.specialityID = s.id
				WHERE uhr.organizationID=@OrganizationId AND  uhr.role_id=4 AND uhr.is_active=1
                GROUP BY u.id";

			var con = _unitOfWork.GetConnection();
			var result = con.Query<UserDAO, string, UserDAO>(query, (user, specialityName) =>
			{
				user.speciality = new Speciality
				{
					id = user.specialityID,
					name = specialityName
				};
				return user;
			}, new { organizationID }, splitOn: "specialityName").ToList();
			return result;
		}
	}
}
