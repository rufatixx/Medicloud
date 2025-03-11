namespace Medicloud.DAL.DAO
{
	public class PortfolioDAO
	{
		public int id { get; set; }
		public string? description { get; set; }
		public int fileId { get; set; }
		public int organizationId { get; set; }
		public bool isActive { get; set; }
		public string categoryIds { get; set; }
	}
}
