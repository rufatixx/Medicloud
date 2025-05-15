using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.ViewModels
{
    public class PoliclinicViewModel
    {
        public List<PatientDAO> Patients { get; set; }
        public int SelectedCardId { get; set; }
		public List<AnamnesisFieldDAO> AnamnesisFields { get; set; }
		public List<AnamnesisDAO> CardAnamnesis { get; set; }
		public string SearchText { get; set; }
	}
}
