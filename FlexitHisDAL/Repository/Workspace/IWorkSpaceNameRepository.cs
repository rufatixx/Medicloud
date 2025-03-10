using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.Workspace
{
	public interface IWorkSpaceNameRepository
	{
		Task<int> AddAsync(string name, int typeId);
	}
}
