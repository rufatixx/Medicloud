using Medicloud.DAL.DAO;

namespace Medicloud.ViewModels
{
	public class CreateOrganizationVM
	{
		public int id { get; set; }
		public string? StaffName { get; set; }
		public string? StaffEmail { get; set; }
		public string? StaffPhoneNumber { get; set; }
		public string? OrgName { get; set; }
		public string? OrgPhoneNumber { get; set; }
		public string? OrgEmail { get; set; }
		public string? OrgAddress {get; set;}
		public int UserId { get; set; }
		public List<CategoryDAO> categories { get; set; }
	}
}
