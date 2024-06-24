using System;
using System.Collections.Generic;
using Medicloud.Models;
using Medicloud.Models;
using Medicloud.Models.DTO;

namespace Medicloud.Models
{
    public class NewPatientViewDTO
    { 
   
       public List<RequestType> requestTypes { get; set; }
        public List<ServiceObj> services { get; set; }
        public List<UserDepRel> departments { get; set; }
        public List<User> personal { get; set; }
        public List<User> referers { get; set; }
        public List<Company> companies { get; set; }
        public int status { get; set; }
        public string requestToken { get; set; }
       
    }
   
   
    
   
  

}
