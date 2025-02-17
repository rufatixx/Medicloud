using System;
using System.Configuration;
using FlexitHisCore;
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
        //Communications communications;
        public PersonalListViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
        public IViewComponentResult Invoke() {
            if (HttpContext.Session.GetInt32("userid") != null)
            {
                UserRepo personal = new UserRepo(ConnectionString);

                return View(personal.GetUserList());

            }
            else {
                return View();
            }
           

        }
       
    }
}

