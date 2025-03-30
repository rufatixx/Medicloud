using Medicloud.DAL.Entities;
using System;
using System.Collections.Generic;

namespace Medicloud.Models.DTO
{
    public class UserDTO
    {
        public UserDAO? personal { get; set; }
        public List<OrganizationDAO>? organizations { get; set; }
        public List<KassaDAO>? kassaList { get; set; }
		public List<RoleDTO> Roles { get; set; }
	}


}
