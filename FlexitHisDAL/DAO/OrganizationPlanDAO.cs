using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.DAO
{
	public class OrganizationPlanDAO
	{

		public int id { get; set; }
		public DateTime createDate { get; set; }
		public DateTime expireDate { get; set; }
		public int planId { get; set; }
		public int organizationId { get; set; }
		public bool isActive { get; set; }
	}
}
