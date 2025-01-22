using Dapper;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.Abstract;

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
			INSERT INTO users
            (name,phoneNumber,email,website,address,latitude,longitude,imagePath,cDate)
			VALUES (@{nameof(OrganizationDAO.name)},
            @{nameof(OrganizationDAO.phoneNumber)},
            @{nameof(OrganizationDAO.email)},
            @{nameof(OrganizationDAO.website)},
            @{nameof(OrganizationDAO.address)},
            @{nameof(OrganizationDAO.latitude)},
            @{nameof(OrganizationDAO.longitude)},
            @{nameof(OrganizationDAO.imagePath)},
            @{nameof(OrganizationDAO.cDate)});

			SELECT LAST_INSERT_ID();";
			var con = _unitOfWork.GetConnection();
			var newId = await con.QuerySingleOrDefaultAsync<int>(AddSql, dao);
			return newId;
		}
	}
}
