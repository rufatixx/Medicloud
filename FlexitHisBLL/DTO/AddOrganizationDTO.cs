
namespace Medicloud.BLL.DTO
{
	public class AddOrganizationDTO
	{
		public string Name { get; set; }
		public List<int> SelectedCategories { get; set; }
		public int UserId { get; set; }
		public string StaffName { get; set; }
		public string StaffEmail { get; set; }
		public string StaffPhoneNumber { get; set; }
		public int MyProperty { get; set; }
	}

	public class UpdateOrganizationDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int UserId { get; set; }
		public string StaffName { get; set; }
		public string StaffEmail { get; set; }
		public string StaffPhoneNumber { get; set; }
		public string FbLink { get; set; }
		public string ILink { get; set; }
		public string WebLink { get; set; }
		public string OnlineShopLink { get; set; }
	}
}
