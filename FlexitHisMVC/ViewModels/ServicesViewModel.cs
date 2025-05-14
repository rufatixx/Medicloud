using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.ViewModels
{
	public class ServicesViewModel
	{
		public List<ServiceObj>	 Services { get; set; }
		public List<RequestTypeDAO>	 RequestTypes { get; set; }
		public List<PatientCardServiceDAO> PatientCardServices { get; set; }
		public int CardId { get; set; }
	}
}
