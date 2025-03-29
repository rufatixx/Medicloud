using Medicloud.Models;

namespace Medicloud.DAL.Repository.Abstract
{
    public interface IServicePriceGroupRepository
    {
        public List<dynamic> GetServicesByPriceGroupID(int priceGroupID);
        public List<dynamic> GetActiveServicesByPriceGroupID(int priceGroupID, long organizationID);
    }
}
