using Medicloud.Models;

namespace Medicloud.DAL.Repository.RequestType
{
	public interface IRequestTypeRepository
	{
		Task<List<RequestTypeDAO>>GetRequestTypes(int organizationId=0);
	}
}
