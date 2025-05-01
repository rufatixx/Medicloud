using Medicloud.Data;
using Medicloud.Models;
using Medicloud.Models.Repository;
using Medicloud.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Medicloud.ViewComponents
{
	public class AddServiceModalViewComponent:ViewComponent
	{
		private readonly string _connectionString;
		private readonly IConfiguration _configuration;
		private readonly ServiceGroupsRepo _serviceGroupsRepo;
		private readonly ServicesRepo _servicesRepo;
		public AddServiceModalViewComponent(IConfiguration configuration)
		{
			_configuration = configuration;
			_connectionString = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
			_serviceGroupsRepo = new(_connectionString);
			_servicesRepo = new(_connectionString);
		}
		public async Task<IViewComponentResult> InvokeAsync(AddServiceModalViewModel vm=null)
		{
			var userID = Convert.ToInt32(HttpContext.User.Claims.FirstOrDefault(c => c.Type == "ID")?.Value ?? "0");
			var organizationID = Convert.ToInt32(HttpContext.Session.GetString("Medicloud_organizationID"));
			if (vm == null)
			{
				vm = new AddServiceModalViewModel();

			}
			vm.Services=_servicesRepo.GetServicesByOrganization(organizationID);
			vm.ServiceGroups = _serviceGroupsRepo.GetGroupsByOrganization(organizationID);
			return View(vm);
		}
	}
}
