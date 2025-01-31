using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.BLL.Services.Staff
{
	public class StaffService:IStaffService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IStaffWorkHoursRepository _workHoursRepository;
		private readonly IStaffRepository _staffRepository;

		public StaffService(IUnitOfWork unitOfWork, IStaffWorkHoursRepository workHoursRepository, IStaffRepository staffRepository)
		{
			_unitOfWork = unitOfWork;
			_workHoursRepository = workHoursRepository;
			_staffRepository = staffRepository;
		}

		public async Task<List<StaffWorkHoursDAO>> GetWorkHours(int staffId)
		{
			using var con =  _unitOfWork.BeginConnection();
			var result = await _workHoursRepository.GetStaffWorkHours(staffId);
			return result;
		}

		public async Task<StaffDAO> GetOwnerStaffByOrganizationId(int organizationId)
		{
			using var con= _unitOfWork.BeginConnection();
			var resul=await _staffRepository.GetOwnerStaffByOrganizationId(organizationId);
			return resul;
		}
	}
}
