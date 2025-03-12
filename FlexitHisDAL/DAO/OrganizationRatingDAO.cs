
using System;

namespace Medicloud.DAL.DAO
{
	public  class OrganizationRatingDAO
	{
		public int id { get; set; }
		public int organizationId { get; set; }
		public int userId { get; set; }
		public int rating { get; set; }
		public DateTime cDate { get; set; }
	}
}
