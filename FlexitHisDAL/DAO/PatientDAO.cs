using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.DAO
{
	public class PatientDAO
	{
		public long ID { get; set; }
		public string name { get; set; }
		public string surname { get; set; }
		public string father { get; set; }
		public int genderID { get; set; }
		public long phone { get; set; }
		public string fin { get; set; }
		public DateTime bDate { get; set; }
	}
}
