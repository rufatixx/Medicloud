using Medicloud.DAL.Entities;

namespace Medicloud.ViewModels
{
	public class NewPatientViewModel
	{
		public List<OrganizationReasonDAO> OrgReasons { get; set; }
		public bool HasHeader { get; set; }
	}
}
