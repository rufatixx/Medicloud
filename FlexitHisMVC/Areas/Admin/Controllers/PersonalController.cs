using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlexitHisCore;
using Medicloud.Areas.Admin.Model;
using Medicloud.BLL.Service;
using Medicloud.BLL.Service.Organization;
using Medicloud.BLL.Services;
using Medicloud.BLL.Services.WorkHour;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Kassa;
using Medicloud.DAL.Repository.Role;
using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Medicloud.Areas.Admin.Controllers
{
	[Area("Admin")]
	public class PersonalController : Controller
	{
		private readonly string _connectionString;
		private readonly IWebHostEnvironment _hostingEnvironment;
		public IConfiguration Configuration;
		private readonly IRoleRepository _roleRepository;
		//Communications communications;

		PersonalPageDTO response;
		IUserService _userService;
		SpecialityRepo specialityRepo;
        IOrganizationService _organizationService;
        IKassaRepo _kassaRepo;
		private readonly IWorkHourService _workHourService;
		public PersonalController(IConfiguration configuration, IKassaRepo kassaRepo, IWebHostEnvironment hostingEnvironment, IRoleRepository roleRepository, IOrganizationService organizationService, IUserService userService, IWorkHourService workHourService)
		{
			Configuration = configuration;
			_connectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_hostingEnvironment = hostingEnvironment;


			response = new PersonalPageDTO();
			_userService = userService;
			specialityRepo = new SpecialityRepo(_connectionString);
			_organizationService = organizationService;
			_roleRepository = roleRepository;
			_kassaRepo = kassaRepo;
			_workHourService = workHourService;
			//communications = new Communications(Configuration, _hostingEnvironment);
		}
		[Authorize]
		public async Task<IActionResult> Index()
		{



			int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

			response.personalList = _userService.GetUserList(organizationId);
			foreach (var user in response.personalList)
			{
				user.roles = await _roleRepository.GetUserRoles(organizationId, user.ID);
			}
			response.specialityList = specialityRepo.GetSpecialities();

            var userId = User.FindFirst("ID")?.Value;
            var currentUser = response.personalList.FirstOrDefault(p => p.ID.ToString() == userId);

			if (currentUser.roles.Select(r => r.id).Contains(2))
			{
				response.organizationList = _organizationService.GetAllOrganizations();
			}
			else if (currentUser.roles.Select(r => r.id).Contains(3))
			{
                response.organizationList = _organizationService.GetOrganizationListWhereUserIsManager(Convert.ToInt32(userId));
            }

			//var orgs = _organizationService.GetAllOrganizations();




			//foreach (var org in orgs)
			//{
			//	Console.WriteLine($"orgID : {org.id}");
			//	var users = _userService.GetUserList(org.id);
			//	if(users!=null && users.Any())
			//	{
			//		foreach (var user in users)
			//		{
			//			Console.WriteLine($"userId : {user.ID}");
			//			await _workHourService.AddOrganizationUserWorkHourAsync(org.id,user.ID);

			//		}
			//		Console.WriteLine($"END ORG");

			//	}

			//}


			return View(response);



		}




		[HttpGet]
		public IActionResult OrganizationsByUser(int personalID)
		{
			if (User.Identity.IsAuthenticated)
			{
				

				return Ok(_organizationService.GetOrganizationsByUser(personalID));

			}

			return Unauthorized();



		}
		[HttpGet]
		public IActionResult DepartmentsByOrganization(int organizationID)
		{

			organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

            if (User.Identity.IsAuthenticated)
			{
				DepartmentsRepo departmentsRepo = new DepartmentsRepo(_connectionString);

				return Ok(departmentsRepo.GetDepartmentsByOrganization(organizationID));

			}

			return Unauthorized();




		}
		[HttpPost]
		public async Task<IActionResult> Add(string name, string surname, string father, int specialityID, string passportSerialNum, string fin, string phone, string email, string bDate, string username, string pwd, List<int> roleIds)
		{
			if (User.Identity.IsAuthenticated)
			{
			

				int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));


				long newUserID = _userService.InsertUser(name, surname, father, specialityID, passportSerialNum, fin, phone, email, bDate, username, pwd, isActive: 1, isRegistered: 1);


				if (newUserID > 0)
				{
					if (roleIds != null && roleIds.Count > 0)
					{
						foreach (int roleId in roleIds)
						{
							int newRelId = await _roleRepository.AddUserRole(organizationId, (int)newUserID, roleId);
						}

					}
					return Ok(newUserID);
				}
				else
				{
					return Ok(0);
				}



				//if (newUserID > 0)
				//{
				//	return Ok(organizationService.LinkOrganizationToUser(newUserID, Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"))));
				//}
				//else
				//{
				//	return Ok(newUserID);//not inserted
				//}



			}

			return Unauthorized();




		}


		[HttpPost]
		public IActionResult AddOrganizationToUser(long userID, int organizationID)
		{
            //organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            if (User.Identity.IsAuthenticated)
			{
				

				return Ok(_organizationService.AddOrganizationToUser(userID, organizationID));

			}

			return Unauthorized();




		}
		[HttpPost]
		public IActionResult RemoveOrganizationFromUser(int userID, int organizationID)
		{
            //organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
            if (User.Identity.IsAuthenticated)
			{
				

				return Ok(_organizationService.UnlinkOrganizationFromUser(userID, organizationID));

			}

			return Unauthorized();



		}
		[HttpGet]
		public IActionResult GetDepartmentsByUser(int userID)
		{
			if (User.Identity.IsAuthenticated)
			{

				UserDepRelRepo userDepRel = new UserDepRelRepo(_connectionString);

				return Ok(userDepRel.GetUserDepartments(userID));

			}

			return Unauthorized();



		}
		[HttpGet]
		public IActionResult GetAllKassaByOrganization(int organizationID)
		{
           
            if (User.Identity.IsAuthenticated)
			{

			

				return Ok(_kassaRepo.GetAllKassaListByOrganization(organizationID));

			}

			return Unauthorized();



		}
		[HttpGet]
		public IActionResult GetUserKassaByOrganization(int organizationID, int userID)
		{
           
            if (User.Identity.IsAuthenticated)
			{

			

				return Ok(_kassaRepo.GetUserKassaByOrganization(organizationID, userID));

			}

			return Unauthorized();


		}
		[HttpPost]
		public IActionResult AddDepToUser(int userID, int depID, bool read_only, bool full_access)
		{
			if (User.Identity.IsAuthenticated)
			{
				UserDepRelRepo departmentsRepo = new UserDepRelRepo(_connectionString);

				return Ok(departmentsRepo.InsertDepToUser(userID, depID, read_only, full_access));

			}

			return Unauthorized();




		}
		[HttpPost]
		public IActionResult AddKassaToUser(int userID, int kassaID, bool read_only, bool full_access)
		{
			if (User.Identity.IsAuthenticated)
			{
				

				return Ok(_kassaRepo.InsertKassaToUser(userID, kassaID, read_only, full_access));

			}

			return Unauthorized();



		}
		[HttpPost]
		public IActionResult RemoveKassaFromUser(int userID, int kassaID)
		{
			if (User.Identity.IsAuthenticated)
			{
			

				return Ok(_kassaRepo.RemoveKassaFromUser(userID, kassaID));

			}

			return Unauthorized();



		}
		[HttpPost]
		public IActionResult RemoveDepFromUser(int userID, int depID)
		{
			if (User.Identity.IsAuthenticated)
			{
				UserDepRelRepo departmentsRepo = new UserDepRelRepo(_connectionString);

				return Ok(departmentsRepo.RemoveDepFromUser(userID, depID));

			}
			return Unauthorized();



		}

		[HttpPost]
		public async Task<IActionResult> UpdateUser(int userID, string name, string surname, string father, int specialityID, string passportSerialNum, string fin, string mobile, string email, string bDate, string username,int isActive, List<int> roleIds)
		{
			if (User.Identity.IsAuthenticated)
			{
				

				int organizationId = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

				long result = _userService.UpdateUser(userID, name, surname, father, specialityID, passportSerialNum, fin, mobile, email, bDate, username);
				var userRoles = await _roleRepository.GetUserRoles(organizationId, userID);
				if (isActive == 0)
				{
					if (userRoles != null && userRoles.Count>0)
					{
						foreach (var role in userRoles)
						{
							await _roleRepository.RemoveUserRole(organizationId, userID, role.id);

						}
					}
				}
				if (roleIds != null)
				{

					if (userRoles != null)
					{

						var rolesToAdd = roleIds.Except(userRoles.Select(r => r.id)).ToList();

	
						var rolesToRemove = userRoles.Select(r => r.id).Except(roleIds).ToList();


						foreach (int roleId in rolesToAdd)
						{

							await _roleRepository.AddUserRole(organizationId, userID, roleId);
						}


						foreach (int roleId in rolesToRemove)
						{
							await _roleRepository.RemoveUserRole(organizationId, userID, roleId);
						}
					}


				}


				return Ok(result);

			}

			return Unauthorized();




		}
		[HttpPost]
		public IActionResult UpdateUserPwd(int userID, string pwd)
		{
			if (User.Identity.IsAuthenticated)
			{
				

				return Ok(_userService.UpdateUser(userID:userID,password: pwd));

			}

			return Unauthorized();




		}

	}
}

