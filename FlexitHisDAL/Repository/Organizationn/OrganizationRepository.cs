using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using System.Text;


namespace Medicloud.DAL.Repository.Organizationn
{
    public class OrganizationRepository:IOrganizationRepository
    {
		private readonly IUnitOfWork _unitOfWork;

		public OrganizationRepository(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<int> AddAsync(OrganizationDAO dao)
		{
			string AddSql = $@"
			INSERT INTO organizations
            (name,phoneNumber,email,website,address,latitude,longitude,imagePath,cDate,ownerId)
			VALUES (@{nameof(OrganizationDAO.name)},
            @{nameof(OrganizationDAO.phoneNumber)},
            @{nameof(OrganizationDAO.email)},
            @{nameof(OrganizationDAO.website)},
            @{nameof(OrganizationDAO.address)},
            @{nameof(OrganizationDAO.latitude)},
            @{nameof(OrganizationDAO.longitude)},
            @{nameof(OrganizationDAO.imagePath)},
            @{nameof(OrganizationDAO.cDate)},
            @{nameof(OrganizationDAO.ownerId)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}

		public async Task<OrganizationDAO?>GetByIdAsync(int id)
		{
			string sql = @"SELECT * FROM organizations WHERE id=@Id";
			var con = _unitOfWork.GetConnection();
			var result=await con.QuerySingleOrDefaultAsync<OrganizationDAO>(sql, new {Id=id});
			return result;
		}



		public async Task RegisterOrganization(int id)
		{
			string sql = $@"
			UPDATE organizations SET isRegistered=1 WHERE id= @Id";
			var con = _unitOfWork.GetConnection();
			int result = await con.ExecuteAsync(sql, new { Id = id });
		}

		public async Task<bool> UpdateAsync(OrganizationDAO dao)
		{
			var query = new StringBuilder("UPDATE organizations SET ");
			var parameters = new DynamicParameters();

			if (!string.IsNullOrEmpty(dao.name))
			{
				query.Append("name = @name, ");
				parameters.Add("@name", dao.name);
			}
			if (!string.IsNullOrEmpty(dao.phoneNumber))
			{
				query.Append("phoneNumber = @phoneNumber, ");
				parameters.Add("@phoneNumber", dao.phoneNumber);
			}
			if (!string.IsNullOrEmpty(dao.email))
			{
				query.Append("email = @email, ");
				parameters.Add("@email", dao.email);
			}
			if (!string.IsNullOrEmpty(dao.website))
			{
				query.Append("website = @website, ");
				parameters.Add("@website", dao.website);
			}
			if (!string.IsNullOrEmpty(dao.address))
			{
				query.Append("address = @address, ");
				parameters.Add("@address", dao.address);
			}
			if (dao.longitude > 0)
			{
				query.Append("longitude = @longitude, ");
				parameters.Add("@longitude", dao.longitude);
			}
			if (dao.latitude > 0)
			{
				query.Append("latitude = @latitude, ");
				parameters.Add("@latitude", dao.latitude);
			}
			if (!string.IsNullOrEmpty(dao.imagePath))
			{
				query.Append("imagePath = @imagePath, ");
				parameters.Add("@imagePath", dao.imagePath);
			}
			if (dao.cDate.HasValue)
			{
				query.Append("cDate = @cDate, ");
				parameters.Add("@cDate", dao.cDate.Value);
			}
			if (dao.workPlaceType > 0)
			{
				query.Append("workPlaceType = @workPlaceType, ");
				parameters.Add("@workPlaceType", dao.workPlaceType);
			}
			if (dao.ownerId > 0)
			{
				query.Append("ownerId = @ownerId, ");
				parameters.Add("@ownerId", dao.ownerId);
			}
			if (dao.teamSizeId > 0)
			{
				query.Append("teamSizeId = @teamSizeId, ");
				parameters.Add("@teamSizeId", dao.teamSizeId);
			}

			// Remove the last comma and space
			if (parameters.ParameterNames.Count() > 0)
			{
				query.Length -= 2;
			}

			query.Append(" WHERE id = @Id");
			parameters.Add("@Id", dao.id);
			var con = _unitOfWork.GetConnection();
			var result=await con.ExecuteAsync(query.ToString(), parameters);
			return result>0;
		}
	}
}
