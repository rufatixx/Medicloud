using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.DAO
{
	public class PatientCardDAO:PatientDAO
	{
		public DateTime cDate { get; set; }
		public long serviceID { get; set; }
		public long patientCardID { get; set; }
		public int totalCardNumbers { get; set; }
		public int finished { get; set; }
		public string note { get; set; }
	}
}
