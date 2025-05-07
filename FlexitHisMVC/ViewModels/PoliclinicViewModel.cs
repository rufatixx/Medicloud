using Medicloud.Models;

namespace Medicloud.ViewModels
{
    public class PoliclinicViewModel
    {
        public List<PatientDocDTO> Patients { get; set; }
        public int SelectedCardId { get; set; }
    }
}
