namespace Medicloud.BLL.Models;

public class AppointmentViewModel
{
    public int id { get; set; }
    public int patient_id { get; set; }
	public string patient_name { get; set; }
	public string patient_surname { get; set; }
	public string patient_phone { get; set; }
	public DateTime start_date { get; set; }
	public DateTime end_date { get; set; }
	public DateTime cDate { get; set; }
	public int service_id { get; set; }
	public string service_name { get; set; }
	public bool is_active { get; set; }
	public int organization_id { get; set; }
}
