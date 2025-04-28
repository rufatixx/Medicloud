using System;
using System.Collections.Generic;
using Medicloud.DAL.Entities;
using Medicloud.Models;
using Medicloud.Models;
using Medicloud.Models.DTO;

namespace Medicloud.Models
{
    public class NewPatientViewDTO
    { 
   
       public List<RequestTypeDAO> requestTypes { get; set; }
        public List<ServiceObj> services { get; set; }
        public List<UserDepRel> departments { get; set; }
        public List<UserDAO> personal { get; set; }
        public List<UserDAO> referers { get; set; }
        public List<CompanyDAO> companies { get; set; }
        public int status { get; set; }
        public string requestToken { get; set; }
       
    }
   
   
    
   
  

}
