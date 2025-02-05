using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;
using Medicloud.WebUI.Enums;

namespace Medicloud.ViewModels
{
	public class CreateOrganizationVM
	{
		public int id { get; set; }
		public string? StaffName { get; set; }
		public string? StaffEmail { get; set; }
		public string? StaffPhoneNumber { get; set; }
		public string? OrgName { get; set; }
		public string? OrgAddress {get; set;}
		public int UserId { get; set; }
		public List<int> SelectedCategories { get; set; }
		public WorkPlaceType WorkPlaceType { get; set; }
		public decimal latitude { get; set; }
		public decimal longitude { get; set; }
		public decimal TravelPrice { get; set; }
		public short TravelPriceType { get; set; }
		public int TravelDistance { get; set; }
		public List<ServiceDAO> Services { get; set; }
		public GetServiceTypesDTO ServiceTypes { get; set; }
		public List<CategoryDAO> Categories { get; set; }
		public bool hasTravel  { get; set; }

	}
}
