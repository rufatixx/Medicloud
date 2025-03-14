namespace Medicloud.DAL.DAO
{
    public class OrganizationDAO
    {
		public int id { get; set; }
		public string? name { get; set; }
		public string? phoneNumber { get; set; }
		public string? website { get; set; }
		public string? address { get; set; }
		public string? email { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public int logoId { get; set; }
		public int coverId { get; set; }
		public DateTime? cDate { get; set; }
		public int ownerId { get; set; }
		public int workPlaceType { get; set; }
		public int teamSizeId { get; set; }
		public bool isRegistered { get; set; }
		public string insLink { get; set; }
		public string fbLink { get; set; }
		public string onlineShopLink { get; set; }
		public string description { get; set; }
		public string staffName { get; set; }
	}
}
