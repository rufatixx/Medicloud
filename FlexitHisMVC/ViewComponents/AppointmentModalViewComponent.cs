using System;
using System.Collections.Specialized;
using Medicloud.BLL.Services;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository.Role;
using Medicloud.Models;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{

    public class AppointmentModalViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        public IUserService _userService;
        public IRoleRepository _roleRepository;
        //Communications communications;
        public AppointmentModalViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IUserService userService,IRoleRepository roleRepository)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
            _userService = userService;
            _roleRepository = roleRepository;
           
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
            var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
            var roles = userRoles.Select(r => r.id);

            List<UserDAO> response = new List<UserDAO>();
            if (roles.Contains(7))
            {
                var allUsers = _userService.GetUserList(organizationID);

                foreach (var user in allUsers)
                {
                    var thisUserRoles = await _roleRepository.GetUserRoles(organizationID, user.ID);
                    if (thisUserRoles.Any(r => r.id == 4))
                    {
                        response.Add(user);
                    }
                }
            }
               

            return View(response);


        }

    }

}

