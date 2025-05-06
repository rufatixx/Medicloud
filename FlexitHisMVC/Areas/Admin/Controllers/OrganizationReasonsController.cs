using Medicloud.Areas.Admin.ViewModels;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository.OrganizationReasons;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrganizationReasonsController : Controller
    {
        private readonly IOrganizationReasonsRepository _organizationReasonsRepository;

        public OrganizationReasonsController(IOrganizationReasonsRepository organizationReasonsRepository)
        {
            _organizationReasonsRepository=organizationReasonsRepository;
        }

        public async Task<IActionResult> Index()
        {
            int organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            var reasons = await _organizationReasonsRepository.GetByOrganizationId(organizationID);
            var vm = new OrganizationReasonsViewModel
            {
                OrganizationReasons= reasons,
            };
            return View(vm);
        }

		[HttpPost]
		public async Task<IActionResult> AddOrUpdate(OrganizationReasonDAO dao)
		{
			int organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			dao.organizationId = organizationID;
			if (dao.id == 0)
			{
				int newId=await _organizationReasonsRepository.AddAsync(dao);
			}
			else
			{
				bool updated=await _organizationReasonsRepository.UpdateAsync(dao);
			}
			return RedirectToAction("Index");
		}
	}
}
