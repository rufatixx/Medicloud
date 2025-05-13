using Medicloud.Models;

namespace Medicloud.ViewModels
{
	public class ClientsViewModel
	{
		public List<PatientDocDTO> Patients { get; set; }
		public string SearchText { get; set; }
	}
}
