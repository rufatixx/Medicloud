
namespace Medicloud.DAL.DAO
{
	public class CommentDAO
	{
		public int id { get; set; }
		public int userId { get; set; }
		public string description { get; set; }
		public DateTime cDate { get; set; }
		public bool isActive { get; set; }
	}
}
