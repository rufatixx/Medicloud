using System;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Role;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509.SigI;

namespace Medicloud.ViewComponents
{

    public class NavbarViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        UserRepo personalDAO;
        OrganizationRepo organizationDAO;
        KassaRepo kassaDAO;
        private readonly string _connectionString;
		private readonly IRoleRepository _roleRepository;
		public NavbarViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository)
		{
			Configuration = configuration;

			_hostingEnvironment = hostingEnvironment;
			//communications = new Communications(Configuration, _hostingEnvironment);
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			personalDAO = new UserRepo(_connectionString);
			organizationDAO = new OrganizationRepo(_connectionString);
			kassaDAO = new KassaRepo(_connectionString);
			_roleRepository = roleRepository;
		}

		public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
        {
			int orgId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			int userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));

			UserDTO obj = new UserDTO();
            obj.personal = new User();
            obj.organizations = new List<Organization>();
            obj.kassaList = new List<Kassa>();

            obj.personal = personalDAO.GetUserByID(userId);
			if (!string.IsNullOrEmpty(obj.personal.imagePath))
			{
				string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", obj.personal.imagePath.TrimStart('/'));
				if (!System.IO.File.Exists(path))
				{
					obj.personal.imagePath = "";
				}
			}

			obj.organizations = organizationDAO.GetOrganizationListByUser(obj.personal.ID);

			obj.Roles=await _roleRepository.GetUserRoles(orgId, userId);
			


			ViewBag.SelectedOrganization = HttpContext.Session.GetString("Medicloud_organizationName");
            return View(obj);
        }
    }
}

