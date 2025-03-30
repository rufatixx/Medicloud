using System;
using System.Configuration;
using FlexitHisCore;
using Medicloud.BLL.Services;
using Medicloud.DAL.Repository;
using Medicloud.Data;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.Areas.Admin.Views.ViewComponents
{
    public class PersonalListViewComponent : ViewComponent 
    {
        private readonly string ConnectionString;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        IUserService _userService;
        //Communications communications;
        public PersonalListViewComponent(IConfiguration configuration,IUserService userService, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
        public IViewComponentResult Invoke() {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
             

                return View(_userService.GetUserList());

            }
            else {
                return View();
            }
           

        }
       
    }
}

