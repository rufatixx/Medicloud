using Medicloud.BLL.Services;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
    [Authorize]
	public class SideNavViewComponent : ViewComponent
	{
		private readonly IWebHostEnvironment _hostingEnvironment;
		public IConfiguration Configuration;
		string _connectionString;
		//Communications communications;
		IUserService _userService;
		private readonly IRoleRepository _roleRepository;
		public SideNavViewComponent(IConfiguration configuration,IUserService userService, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository)
		{
			Configuration = configuration;

			_hostingEnvironment = hostingEnvironment;

			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;

			_userService = userService;
			_roleRepository = roleRepository;
			//communications = new Communications(Configuration, _hostingEnvironment);
		}
		public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
		{
			var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
			int orgId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

			UserDAO user = new UserDAO();
			if (userID > 0)
			{
				user = _userService.GetUserById(Convert.ToInt32(userID));

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

