using Medicloud.BLL.Models;
using Medicloud.BLL.Services.Abstract;
using Medicloud.DAL.Repository.Abstract;
using Medicloud.DAL.Repository.Role;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Domain;
using Medicloud.Models.Repository;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Controllers
{
    [Authorize]
    public class PrescriptionsController : Controller
    {
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private PriceGroupCompanyRepository priceGroupCompanyRepository;
        private IServicePriceGroupRepository _servicePriceGroupRepository;
        PatientCardRepo patientCardRepo;
        PatientCardServiceRelRepo patientCardServiceRelRepo;
        PatientDiagnoseRel patientDiagnoseRel;
        ServicesRepo servicesRepo;
        PatientRepo patientRepo;
        RequestTypeRepo requestTypeDAO;
        private readonly IPatientCardService _patientCardService;
        private readonly IRoleRepository _roleRepository;
        public PrescriptionsController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment,IRoleRepository roleRepository, IPatientCardService patientCardService, IServicePriceGroupRepository servicePriceGroupRepository)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            priceGroupCompanyRepository = new PriceGroupCompanyRepository(ConnectionString);
            _servicePriceGroupRepository = servicePriceGroupRepository;
            patientCardRepo = new PatientCardRepo(ConnectionString);
            patientCardServiceRelRepo = new PatientCardServiceRelRepo(ConnectionString);
            patientDiagnoseRel = new PatientDiagnoseRel(ConnectionString);
            servicesRepo = new ServicesRepo(ConnectionString);
            patientRepo = new PatientRepo(ConnectionString);
            requestTypeDAO = new RequestTypeRepo(ConnectionString);
            _patientCardService=patientCardService;
            _roleRepository = roleRepository;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(string patientFullName,int patientID)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
                var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
                var userRoles = await _roleRepository.GetUserRoles(organizationID, userID);
                var roles = userRoles.Select(r => r.id);
                List<PatientDocDTO> response = new List<PatientDocDTO>();
                if (roles.Contains(7))
                {
                    response = await _patientCardService.GetAllPatientsCards(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")), patientID);

                    
                   
                    ViewBag.patientFullname = patientFullName;
                }
                else if (roles.Contains(4))
                {
                    response = await _patientCardService.GetAllPatientsCards(Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID")), patientID);
                    ViewBag.patientFullname = patientFullName;
                }
                return View(response);
           
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }



        [HttpPost]

        public IActionResult AddPrescription(int requestTypeID,long patientID,long cardID, int serviceID, string note)
        {
            if (User.Identity.IsAuthenticated)
            {

                try
                {


                    var userID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_userID"));
                    var organizationID = Convert.ToInt64(HttpContext.Session.GetString("Medicloud_organizationID"));

                    if (cardID==0)
                    {
                        cardID = patientCardRepo.CreatePatientCard(requestTypeID, userID, patientID, organizationID, serviceID,note:note);
                    }

                       
                        var serviceInserted = patientCardServiceRelRepo.InsertServiceToPatientCard(cardID, serviceID, 0,0, userID);
                        if (cardID == 0 || serviceInserted == false)
                        {
                            return BadRequest("Xəstə kartını daxil etmək mümkün olmadı.");
                        }

                   

                    return Ok(cardID);
                }
                catch (Exception ex)
                {
                    // Handle the exception and return an appropriate response
                    return StatusCode(StatusCodes.Status500InternalServerError, "Sorğunu emal edərkən xəta baş verdi.");
                }
            }
            return Unauthorized();
        }





    }
}

