namespace Medicloud.DAL.Entities
{
	public class OrganizationReasonDAO
	{
		public int id { get; set; }
		public string name { get; set; }
		public int organizationId { get; set; }
		public bool isActive { get; set; }
	}
}
