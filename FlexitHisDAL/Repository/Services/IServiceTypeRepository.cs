using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.Services
{
	public interface IServiceTypeRepository
	{
		Task<List<ServiceTypeDAO>> GetAllTypes();
		Task<List<TempDAO>> GetTypeCategories();
	}
}
