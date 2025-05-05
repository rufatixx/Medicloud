namespace Medicloud.DAL.Entities
{
	public class OrganizationRefererDAO
	{
		public int id { get; set; }
		public string name { get; set; }
		public int organizationId { get; set; }
		public bool isActive { get; set; }
	}
}
