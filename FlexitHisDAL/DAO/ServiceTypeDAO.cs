using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml.Linq;

namespace Medicloud.DAL.DAO
{
	public class ServiceTypeDAO
	{
		public int id { get; set; }
		public string name { get; set; }
		public int typeCategoryId { get; set; }
		public bool  isActive { get; set; }
	}
}
