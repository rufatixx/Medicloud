using System;
using Medicloud.Models.Domain;
using Medicloud.Models.DTO;

namespace Medicloud.Models
{
    public class PatientCardDTO:Patient
    {
       
        public long CardID { get; set; }
        public string cDate { get; set; }
        public List<PatientServiceDTO> Services { get; set; }
    }
}
