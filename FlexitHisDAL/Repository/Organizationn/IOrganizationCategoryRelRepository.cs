namespace Medicloud.DAL.Repository.Organizationn
{
    public interface IOrganizationCategoryRelRepository
    {
		Task<int> AddAsync(int organizationId, int categoryId);
    }
}
