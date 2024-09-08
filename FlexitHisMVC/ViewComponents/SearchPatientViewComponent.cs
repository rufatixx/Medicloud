using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
    public class SearchPatientViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke() 
        {

            return View();
        }
    }
}
