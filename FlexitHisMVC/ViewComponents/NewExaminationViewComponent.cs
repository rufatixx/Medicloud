using Medicloud.BLL.Services;
using Medicloud.BLL.Services.Abstract;
using Medicloud.DAL.Repository.RequestType;
using Medicloud.Data;
using Medicloud.Models.Repository;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace Medicloud.ViewComponents
{
	public class NewExaminationViewComponent : ViewComponent
	{
		private readonly IUserService _userService;
		private readonly IRequestTypeService _requestTypeService;
		private readonly CompanyRepo _companyRepo;
		private readonly string _connectionString;
		private readonly IConfiguration _configuration;
		private readonly ServicesRepo _servicesRepo;
		public NewExaminationViewComponent(IUserService userService, IRequestTypeService requestTypeService, IConfiguration configuration)
		{
			_userService = userService;
			_requestTypeService = requestTypeService;
			_configuration = configuration;
			_connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_companyRepo=new(_connectionString);
			_servicesRepo = new(_connectionString);
		}

		public async Task<IViewComponentResult> InvokeAsync(ExaminationViewModel vm=null)
		{
			var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
			var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));

			if (vm == null)
			{
				vm=new ExaminationViewModel();
			}
			//var doctors = await _userService.GetDoctorUsersByOrganization(organizationID);
			vm.RequestTypes = await _requestTypeService.GetRequestTypesAsync(organizationID);
			vm.Companies= _companyRepo.GetActiveCompanies(organizationID);
			vm.Services=_servicesRepo.GetServicesByOrganization(organizationID);
			//var vm = new ExaminationViewModel
			//{
			//	Doctors = doctors,
			//	RequestTypes = requestTypes,
			//	Companies = companies,
			//	Services = services
				
			//};
			return View(vm);
		}
	}
}
