using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.DTO
{
	public class GetServiceTypesDTO
	{
		public List<TempDAO> ServiceTypeCategories { get; set; }
		public List<ServiceTypeDAO> ServiceTypes { get; set; }
	}
}
