using System;
using System.Collections.Generic;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models;
using FlexitHisMVC.Models.DTO;

namespace FlexitHisMVC.Models
{
    public class NewPatientViewDTO
    { 
   
       public List<RequestType> requestTypes { get; set; }
        public List<Service> services { get; set; }
        public List<UserDepRel> departments { get; set; }
        public List<Personal> personal { get; set; }
        public List<Personal> referers { get; set; }
        public int status { get; set; }
        public string requestToken { get; set; }
       
    }
   
   
    
   
  

}
