using System;
using FlexitHisMVC.Models.Domain;

namespace FlexitHisMVC.Models
{
    public class PatientKassaDTO:Patient
    {
       
        public string serviceName { get; set; }
        public double servicePrice { get; set; }
    }
}
