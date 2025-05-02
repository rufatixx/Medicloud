using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.ViewModels
{
	public class ReceptionViewModel
	{
		public List<DepartmentDAO> Departments { get; set; }
		public List<UserDAO> Doctors { get; set; }
	}
}
