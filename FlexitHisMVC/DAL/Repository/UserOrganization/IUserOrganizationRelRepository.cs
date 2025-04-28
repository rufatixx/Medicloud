using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.UserOrganization
{
	public interface IUserOrganizationRelRepository
	{
		Task<List<UserDAO>> GetDoctorsByOrganization(int organizationID);
	}
}
