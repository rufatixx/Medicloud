using System;
using System.Collections.Specialized;
using Medicloud.DAL.Repository.Kassa;
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
        IKassaRepo _kassaRepo;
        public KassaSelectorViewComponent(IConfiguration configuration,IKassaRepo kassaRepo, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
            //communications = new Communications(Configuration, _hostingEnvironment);
            _connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _kassaRepo = kassaRepo;
        }

        public async Task<IViewComponentResult> InvokeAsync(/* параметры, если необходимы */)
        {
            var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            var userId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
            var kassaList = _kassaRepo.GetUserKassaByOrganization(organizationID, userId);
            Console.WriteLine(kassaList?.Count>0?kassaList.Count.ToString():"nulldu");
            var selectedKassaName = HttpContext.Session.GetString("Medicloud_kassaName");
         

           
                // Используем сохранённое в куки имя, если касса существует
                ViewBag.SelectedKassaName = selectedKassaName ?? kassaList.FirstOrDefault()?.name;
           

            return View(kassaList);
        }
    }

}

