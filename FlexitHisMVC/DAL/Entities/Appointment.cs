namespace Medicloud.DAL.Entities;

public class Appointment
{
    public int id { get; set; }
    public int patient_id { get; set; }
    public int service_id { get; set; }
    public long organization_id { get; set; }
    public DateTime start_date { get; set; }
    public DateTime end_date { get; set; }
    public bool is_active { get; set; }
}
