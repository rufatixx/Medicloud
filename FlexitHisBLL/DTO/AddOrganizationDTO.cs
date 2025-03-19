
namespace Medicloud.BLL.DTO
{
	public class AddOrganizationDTO
	{
		public string Name { get; set; }
		public List<int> SelectedCategories { get; set; }
		public int UserId { get; set; }
		public string StaffName { get; set; }
		public string StaffEmail { get; set; }
		public string PhoneNumber { get; set; }
	}

	public class UpdateOrganizationDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int UserId { get; set; }
		public string? StaffName { get; set; }
		public string? StaffEmail { get; set; }
		public string? PhoneNumber { get; set; }
		public string? FbLink { get; set; }
		public string? ILink { get; set; }
		public string? WebLink { get; set; }
		public string? OnlineShopLink { get; set; }
		public int WorkPlaceType { get; set; }
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
		public string? Address { get; set; }
		public int TeamSizeId { get; set; }
		public string Description { get; set; }
	}
	public class OrganizationDTO
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int UserId { get; set; }
		public int LogoId { get; set; }
		public string? StaffName { get; set; }
		public string? StaffEmail { get; set; }
		public string? StaffPhoneNumber { get; set; }
		public string? FbLink { get; set; }
		public string? ILink { get; set; }
		public string? WebLink { get; set; }
		public string? OnlineShopLink { get; set; }
		public int WorkPlaceType { get; set; }
		public decimal Latitude { get; set; }
		public decimal Longitude { get; set; }
		public string? Address { get; set; }
		public int TeamSizeId { get; set; }
		public string Description { get; set; }
	}
}

