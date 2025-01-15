using Medicloud.DAL.Entities;
using System;
using System.Collections.Generic;

namespace Medicloud.Models.DTO
{
    public class UserDTO
    {
        public User? personal { get; set; }
        public List<Organization>? organizations { get; set; }
        public List<Kassa>? kassaList { get; set; }
		public List<RoleDTO> Roles { get; set; }
	}


}
