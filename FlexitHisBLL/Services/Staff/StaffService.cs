using Medicloud.BLL.DTO;
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

		public async Task<List<StaffWorkHoursDTO>> GetWorkHours(int staffId)
		{
			using var con =  _unitOfWork.BeginConnection();
			var workhours = await _workHoursRepository.GetStaffWorkHours(staffId);
			var result=new List<StaffWorkHoursDTO>();
			foreach (var item in workhours)
			{
				Console.WriteLine(item.id.ToString());
				result.Add(new()
				{
					id=item.id,
					startTime=item.startTime,
					endTime=item.endTime,
					dayOfWeek=item.dayOfWeek,
					Breaks= await _workHoursRepository.GetStaffBreaksWithWorkHourId(item.id),
				});
			}
			return result;
		}

		public async Task<StaffDAO> GetOwnerStaffByOrganizationId(int organizationId)
		{
			using var con= _unitOfWork.BeginConnection();
			var result=await _staffRepository.GetOwnerStaffByOrganizationId(organizationId);
			return result;
		}
		public async Task<bool> UpdateStaffAsync(StaffDAO dao)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _staffRepository.UpdateStaffAsync(dao);
			return result;
		}
	}
}
