namespace Medicloud.BLL.Models;

public class AppointmentPagedResult
{
    public List<AppointmentViewModel> Appointments { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
}
