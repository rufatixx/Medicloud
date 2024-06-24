using System;
using System.Collections.Specialized;
using System.Net.NetworkInformation;
using Medicloud.DAL.Repository;
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
        public NavbarViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            personalDAO = new UserRepo(_connectionString);
            organizationDAO = new OrganizationRepo(_connectionString);
            kassaDAO = new KassaRepo(_connectionString);

        }

        public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
        {
            UserDTO obj = new UserDTO();
            obj.personal = new User();
            obj.organizations = new List<Organization>();
            obj.kassaList = new List<Kassa>();

            obj.personal = personalDAO.GetUserByID(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID")));

          
            obj.organizations = organizationDAO.GetOrganizationListByUser(obj.personal.ID);

         


            ViewBag.SelectedOrganization = HttpContext.Session.GetString("Medicloud_organizationName");
            return View(obj);
        }
    }
}

