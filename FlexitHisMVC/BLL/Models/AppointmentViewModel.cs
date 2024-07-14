namespace Medicloud.BLL.Models;

public class AppointmentViewModel
{
    public int id;
    public int patient_id;
    public string patient_name;
    public string patient_surname;
    public DateTime start_date;
    public DateTime end_date;
    public DateTime cDate;
    public int service_id;
    public string service_name;
    public bool is_active;
    public int organization_id;
}
