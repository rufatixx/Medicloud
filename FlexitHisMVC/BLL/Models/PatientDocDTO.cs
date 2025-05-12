using System;
using Medicloud.Models.Domain;

namespace Medicloud.Models
{
    public class PatientDocDTO:Patient
    {
      
        public DateTime cDate { get; set; }
        public long serviceID { get; set; }
        public long patientCardID { get; set; }
        public int totalCardNumbers { get; set; }
        public int finished { get; set; }
        public string note { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
		public bool isActive { get; set; }
	}
}
