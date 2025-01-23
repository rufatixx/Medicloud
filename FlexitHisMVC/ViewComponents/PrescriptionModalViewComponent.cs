
using Medicloud.BLL.Services.RequestType;
using Medicloud.BLL.Services.Services;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
    public class PrescriptionModalViewComponent:ViewComponent
    {
        private readonly IServicesService _service;
        private readonly IRequestTypeService _requestTypeService;

        public PrescriptionModalViewComponent(IServicesService service, IRequestTypeService requestTypeService)
        {
            _service=service;
            _requestTypeService=requestTypeService;
        }
        public async Task<IViewComponentResult>InvokeAsync()
        {
            ViewBag.services =  await _service.GetServicesByOrganizationAsync(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")));

            ViewBag.requestTypes = await _requestTypeService.GetRequestTypesAsync();
            return View();
        }
    }

}
