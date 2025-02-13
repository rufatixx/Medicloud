using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Medicloud.DAL.DAO
{
	public class PlanDAO
	{

		public int id { get; set; }
		public string name { get; set; }
		public int teamSizeId { get; set; }
		public decimal price { get; set; }
		public int durationDay { get; set; }
	}
}
