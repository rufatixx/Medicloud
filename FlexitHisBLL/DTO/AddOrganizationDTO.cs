using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.DTO
{
	public class AddOrganizationDTO
	{
		public string Name { get; set; }
		public List<int> SelectedCategories { get; set; }
		public int UserId { get; set; }
		public string StaffName { get; set; }
		public string StaffEmail { get; set; }
		public string StaffPhoneNumber { get; set; }
		public int MyProperty { get; set; }
	}
}
