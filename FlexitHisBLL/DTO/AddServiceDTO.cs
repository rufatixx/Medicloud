using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.DTO
{
	public class AddServiceDTO
	{
		public string name { get; set; }
		public decimal price { get; set; }
		public int time { get; set; }
		public bool isMobile { get; set; }
		public int typeId { get; set; }
		public int organizationId { get; set; }
	}
}
