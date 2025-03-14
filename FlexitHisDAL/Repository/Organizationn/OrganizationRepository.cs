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
            (name,phoneNumber,email,website,address,latitude,longitude,logoId,cDate,ownerId)
			VALUES (@{nameof(OrganizationDAO.name)},
            @{nameof(OrganizationDAO.phoneNumber)},
            @{nameof(OrganizationDAO.email)},
            @{nameof(OrganizationDAO.website)},
            @{nameof(OrganizationDAO.address)},
            @{nameof(OrganizationDAO.latitude)},
            @{nameof(OrganizationDAO.longitude)},
            @{nameof(OrganizationDAO.logoId)},
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
			if (dao.logoId>0 )
			{
				query.Append("logoId = @logoId, ");
				parameters.Add("@logoId", dao.logoId);
			}
			if (dao.coverId > 0)
			{
				query.Append("coverId = @coverId, ");
				parameters.Add("@coverId", dao.coverId);
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

			if (dao.isRegistered)
			{
				query.Append("isRegistered = @isRegistered, ");
				parameters.Add("@isRegistered", 1);
			}
			if (!string.IsNullOrEmpty(dao.insLink))
			{
				query.Append("insLink = @insLink, ");
				parameters.Add("@insLink", dao.insLink);
			}
			if (!string.IsNullOrEmpty(dao.fbLink))
			{
				query.Append("fbLink = @fbLink, ");
				parameters.Add("@fbLink", dao.fbLink);
			}
			if (!string.IsNullOrEmpty(dao.onlineShopLink))
			{
				query.Append("onlineShopLink = @onlineShopLink, ");
				parameters.Add("@onlineShopLink", dao.onlineShopLink);
			}
			if (!string.IsNullOrEmpty(dao.description))
			{
				query.Append("description = @description, ");
				parameters.Add("@description", dao.description);
			}
			// Remove the last comma and space
			if (parameters.ParameterNames.Count() > 0)
			{
				query.Length -= 2;
			}
			query.Append(" WHERE id = @Id");
			parameters.Add("@Id", dao.id);

			Console.WriteLine(query);
			var con = _unitOfWork.GetConnection();
			var result=await con.ExecuteAsync(query.ToString(), parameters);
			return result>0;
		}

		public async Task UpdateLogoId(int organizationId, int logoId)
		{
			var con=_unitOfWork.GetConnection();
			await con.ExecuteAsync(@"UPDATE organizations SET logoId=@LogoId WHERE id=@OrganizationId", new { OrganizationId = organizationId, LogoId = logoId });
		}

		public async Task UpdateCoverId(int organizationId, int coverId)
		{
			var con = _unitOfWork.GetConnection();
			await con.ExecuteAsync(@"UPDATE organizations SET coverId=@CoverId WHERE id=@OrganizationId", new { OrganizationId = organizationId, CoverId = coverId });
		}

		public async Task<List<OrganizationDAO>> GetUserOrganizations(int userId)
		{
			string sql = @"SELECT o.id,o.name,o.logoId ,s.name AS staffName
						FROM organizations o
						LEFT JOIN staff s ON s.organizationId=o.id AND s.isActive=1
						WHERE o.isRegistered=1 AND s.userId=@UserId";

			var con = _unitOfWork.GetConnection();
			var result=await con.QueryAsync<OrganizationDAO>(sql,new {UserId=userId});
			return result.ToList();
		}
	}
}
