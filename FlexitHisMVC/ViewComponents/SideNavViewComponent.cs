using System;
using Medicloud.Data;
using Medicloud.Models;
using System.Net.NetworkInformation;
using Medicloud.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Role;

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
		private readonly IRoleRepository _roleRepository;
		public SideNavViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository)
		{
			Configuration = configuration;

			_hostingEnvironment = hostingEnvironment;

			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;

			personalDAO = new UserRepo(_connectionString);
			_roleRepository = roleRepository;
			//communications = new Communications(Configuration, _hostingEnvironment);
		}
		public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
		{
			var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
			int orgId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

			User user = new User();
			if (userID > 0)
			{
				user = personalDAO.GetUserByID(Convert.ToInt32(userID));

				if (!string.IsNullOrEmpty(user.imagePath))
				{
					string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.imagePath.TrimStart('/'));
					if (!System.IO.File.Exists(path))
					{
						user.imagePath = "";
					}
				}
			}
			var roles = await _roleRepository.GetUserRoles(orgId, userID);
			user.roles = roles;
			return View(user);
		}

	}
}

