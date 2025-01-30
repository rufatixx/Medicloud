namespace Medicloud.DAL.DAO
{
	public class StaffDAO
	{
		public int id { get; set; }
		public string? name { get; set; }
		public string? phoneNumber { get; set; }
		public string? email { get; set; }
		public int organizationId { get; set; }
		public int permissionLevelId { get; set; }
		public int userId { get; set; }
	}
}
