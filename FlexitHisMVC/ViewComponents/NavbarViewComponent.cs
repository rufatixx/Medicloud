using System;
using Microsoft.AspNetCore.Mvc;

namespace FlexitHisMVC.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        public NavbarViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
        public IViewComponentResult Invoke(int id)
        {

            return View();


        }

    }
}

