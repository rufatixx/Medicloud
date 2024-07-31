namespace Medicloud.BLL.Models;

public class AddAppointmentDto
{
	public int Id { get; set; }
    public int PatientId { get; set; }
    public int ServiceId { get; set; }
    public long OrganizationID { get; set; }
    public DateTime MeetingDate { get; set; }
    public TimeSpan Time { get; set; }
	public int UserId { get; set; }
	public long? PhoneNumber { get; set; }
}
