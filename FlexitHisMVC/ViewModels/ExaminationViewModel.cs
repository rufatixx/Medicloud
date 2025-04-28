using Medicloud.DAL.Entities;
using Medicloud.Models;

namespace Medicloud.ViewModels
{
	public class ExaminationViewModel
	{
		public List<UserDAO> Doctors { get; set; }
		public List<UserDAO> Referers { get; set; }
		public List<RequestTypeDAO> RequestTypes { get; set; }
		public List<CompanyDAO> Companies { get; set; }
		public List<ServiceObj> Services { get; set; }
	}
}
