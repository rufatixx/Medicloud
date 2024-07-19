namespace Medicloud.DAL.Entities;

public class UserPlan
{
	public int id { get; set; }
	public int user_id { get; set; }
	public int plan_id { get; set; }
	public DateTime	cDate { get; set; }
	public DateTime? expire_date { get; set; }
}
