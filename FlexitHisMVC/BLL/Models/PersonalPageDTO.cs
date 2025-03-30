using System;
using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.Areas.Admin.Model
{
    public class PersonalPageDTO
    {
        public List<UserDAO> personalList { get; set; }
        public List<Speciality> specialityList { get; set; }
        public List<OrganizationDAO> organizationList { get; set; }
      
    }
}

