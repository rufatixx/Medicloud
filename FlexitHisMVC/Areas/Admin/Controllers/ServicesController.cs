using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisMVC.Data;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FlexitHisMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServicesController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public IActionResult InsertServiceGroup(long buildingID, string depName, int depTypeID)
        //{
        //    if (HttpContext.Session.GetInt32("userid") != null)
        //    {
        //        DepartmentsRepo insert = new DepartmentsRepo(ConnectionString);
        //        var response = insert.InsertDepartments(buildingID, depName, depTypeID);


        //        return Ok(response);

        //    }
        //    else
        //    {
        //        return Unauthorized();
        //    }


        //}
    }
}

