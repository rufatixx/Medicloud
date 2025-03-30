using Medicloud.BLL.Service.Organization;
using Medicloud.BLL.Services;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Kassa;
using Medicloud.DAL.Repository.Role;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace Medicloud.ViewComponents
{
	
    public class NavbarViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        IUserService _userService;
        IOrganizationService _organizationService;
        IKassaRepo _kassaRepo;
        private readonly string _connectionString;
		private readonly IRoleRepository _roleRepository;
		public NavbarViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository,IOrganizationService organizationService,IUserService userService,IKassaRepo kassaRepo)
		{
			Configuration = configuration;

			_hostingEnvironment = hostingEnvironment;
			//communications = new Communications(Configuration, _hostingEnvironment);
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_userService = userService;
			_organizationService = organizationService;
			_kassaRepo = kassaRepo;
			_roleRepository = roleRepository;
		}

		public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
        {
			
			int orgId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			int userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));

			UserDTO obj = new UserDTO();
            obj.personal = new UserDAO();
            obj.organizations = new List<OrganizationDAO>();
            obj.kassaList = new List<KassaDAO>();

            obj.personal = _userService.GetUserById(userId);
			if (!string.IsNullOrEmpty(obj.personal.imagePath))
			{
				string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", obj.personal.imagePath.TrimStart('/'));
				if (!System.IO.File.Exists(path))
				{
					obj.personal.imagePath = "";
				}
			}

			obj.organizations = _organizationService.GetOrganizationsByUser(obj.personal.ID);

			obj.Roles=await _roleRepository.GetUserRoles(orgId, userId);
			


			ViewBag.SelectedOrganization = HttpContext.Session.GetString("Medicloud_organizationName");
			ViewBag.SelectedOrganizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            return View(obj);
        }
    }
}

