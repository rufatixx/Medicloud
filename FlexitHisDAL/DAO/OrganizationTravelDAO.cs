using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.DAO
{
	public class OrganizationTravelDAO
	{
		public int organizationId { get; set; }
		public decimal fee { get; set; }
		public decimal distance { get; set; }
		public short feeType { get; set; }
	}
}
