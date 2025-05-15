using Medicloud.Models;

namespace Medicloud.ViewModels
{
	public class PrescriptionsViewModel
	{
		public int PatientId { get; set; }
		public string SearchText { get; set; }
		public List<PatientDocDTO> PatientCards  { get; set; }
		public bool isDoctor { get; set; }
	}
}
