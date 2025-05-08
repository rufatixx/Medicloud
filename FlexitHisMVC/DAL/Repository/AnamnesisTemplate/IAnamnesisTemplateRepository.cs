using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.AnamnesisTemplate
{
	public interface IAnamnesisTemplateRepository
	{
		Task<int> AddAsync(AnamnesisTemplateDAO dao);
	}
}
