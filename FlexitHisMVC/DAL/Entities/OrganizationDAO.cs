using System;
namespace Medicloud.DAL.Entities
{
	public class OrganizationDAO
	{

		public int id { get; set; }
		public int organizationID { get; set; }
		public int userID { get; set; }
		public int ownerId { get; set; }
		public string organizationName { get; set; }
		public List<RoleDTO> Roles { get; set; }

	}
}

