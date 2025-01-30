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
		public string? imagePath { get; set; }
		public DateTime? cDate { get; set; }
		public int ownerId { get; set; }
		public int workPlaceType { get; set; }

	}
}
