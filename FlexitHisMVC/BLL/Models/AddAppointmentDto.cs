namespace Medicloud.BLL.Models;

public class AddAppointmentDto
{
    public int PatientId { get; set; }
    public int ServiceId { get; set; }
    public DateTime MeetingDate { get; set; }
    public TimeSpan Time { get; set; }
}
