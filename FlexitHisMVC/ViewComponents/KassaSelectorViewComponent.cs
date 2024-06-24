using System;
using System.Collections.Specialized;
using Medicloud.Data;
using Medicloud.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.X509.SigI;

namespace Medicloud.ViewComponents
{

    public class KassaSelectorViewComponent : ViewComponent
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        //Communications communications;
        KassaRepo kassaRepo;
        private readonly string _connectionString;
        public KassaSelectorViewComponent(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            kassaRepo = new KassaRepo(_connectionString);
        }

        public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
        {
            var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            var userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
            var kassaList = kassaRepo.GetUserKassaByOrganization(organizationID, userId);

            var selectedKassaName = HttpContext.Session.GetString("Medicloud_kassaName");
         

           
                // Используем сохранённое в куки имя, если касса существует
                ViewBag.SelectedKassaName = selectedKassaName ?? kassaList.FirstOrDefault()?.name;
           

            return View(kassaList);
        }
    }

}

