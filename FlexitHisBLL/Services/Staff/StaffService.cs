using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Staff;
using Medicloud.DAL.Repository.WorkHour;

namespace Medicloud.BLL.Services.Staff
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWorkHoursRepository _workHoursRepository;
        private readonly IStaffRepository _staffRepository;

        public StaffService(IUnitOfWork unitOfWork, IWorkHoursRepository workHoursRepository, IStaffRepository staffRepository)
        {
            _unitOfWork = unitOfWork;
            _workHoursRepository = workHoursRepository;
            _staffRepository = staffRepository;
        }

        public async Task<StaffDAO> GetOwnerStaffByOrganizationId(int organizationId)
        {
            using var con = _unitOfWork.BeginConnection();
            var result = await _staffRepository.GetOwnerStaffByOrganizationId(organizationId);
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
