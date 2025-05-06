using Medicloud.DAL.Repository.OrganizationReasons;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
	public class NewPatientViewComponent : ViewComponent
	{
		private readonly IOrganizationReasonsRepository _organizationReasonsRepository;

		public NewPatientViewComponent(IOrganizationReasonsRepository organizationReasonsRepository)
		{
			_organizationReasonsRepository = organizationReasonsRepository;
		}

		public async Task<IViewComponentResult> InvokeAsync()
		{
			int organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

			var orgReasons = await _organizationReasonsRepository.GetByOrganizationId(organizationID, true);
			var vm = new NewPatientViewModel
			{
				OrgReasons = orgReasons
			};
			return View(vm);
		}
	}
}
