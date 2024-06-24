using System;
using Medicloud.Data;
using Medicloud.Models;
using System.Net.NetworkInformation;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Medicloud.DAL.Repository;

namespace Medicloud.ViewComponents
{
    [Authorize]
    public class SideNavViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        string _connectionString;
        //Communications communications;
        UserRepo personalDAO;
        public SideNavViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;

            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;

            personalDAO = new UserRepo(_connectionString);
            //communications = new Communications(Configuration, _hostingEnvironment);
        }
        public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
        {
            var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));


            User user = new User();
            if (userID>0)
            {
               user = personalDAO.GetUserByID(Convert.ToInt32(userID));
            }
           

            return View(user);
        }

    }
}

