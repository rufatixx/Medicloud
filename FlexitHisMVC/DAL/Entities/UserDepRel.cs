using System;
namespace Medicloud.Models
{
	public class UserDepRel
	{
        public long ID { get; set; }
        public int depID { get; set; }
        public int userID { get; set; }
        public int readOnly { get; set; }
        public int fullAccess { get; set; }
        public int isActive { get; set; }
        public DateTime cDate { get; set; }
    }
}

