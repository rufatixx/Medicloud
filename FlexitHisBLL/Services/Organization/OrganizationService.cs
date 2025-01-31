using Medicloud.BLL.DTO;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.Organizationn;
using Medicloud.DAL.Repository.OrganizationTravelRel;
using Medicloud.DAL.Repository.Staff;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Medicloud.BLL.Services.Organization
{
	public class OrganizationService : IOrganizationService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IOrganizationRepository _organizationRepository;
		private readonly IStaffRepository _staffRepository;
		private readonly IOrganizationCategoryRelRepository _organizationCategoryRelRepository;
		private readonly IOrganizationTravelRelRepository _organizationTravelRelRepository;
		private readonly IStaffWorkHoursRepository _staffWorkHoursRepository;
		public OrganizationService(IUnitOfWork unitOfWork, IOrganizationRepository organizationRepository, IStaffRepository staffRepository, IOrganizationCategoryRelRepository organizationCategoryRelRepository, IOrganizationTravelRelRepository organizationTravelRelRepository, IStaffWorkHoursRepository staffWorkHoursRepository)
		{
			_unitOfWork = unitOfWork;
			_organizationRepository = organizationRepository;
			_staffRepository = staffRepository;
			_organizationCategoryRelRepository = organizationCategoryRelRepository;
			_organizationTravelRelRepository = organizationTravelRelRepository;
			_staffWorkHoursRepository = staffWorkHoursRepository;
		}

		public async Task<int> AddAsync(AddOrganizationDTO dto)
		{

			var orgDAO = new OrganizationDAO
			{
				name = dto.Name
			};
			using var con = _unitOfWork.BeginConnection();
			int newOrgId = await _organizationRepository.AddAsync(orgDAO);

			foreach (var categoryId in dto.SelectedCategories)
			{
				Console.WriteLine($"categoryID{categoryId}");
				await _organizationCategoryRelRepository.AddAsync(newOrgId, categoryId);
			}

			var staffDAO = new StaffDAO
			{
				name = dto.StaffName,
				email = dto.StaffEmail,
				phoneNumber = dto.StaffPhoneNumber,
				permissionLevelId = 1,
				organizationId = newOrgId,
				userId = dto.UserId,
			};
			int staffId = await _staffRepository.AddAsync(staffDAO);
			TimeOnly defaultStartTime = new TimeOnly(9, 0);
			TimeOnly defaultEndTime = new TimeOnly(18, 0);
			var workHours = new List<StaffWorkHoursDAO>
			{
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 1, startTime =defaultStartTime.ToString("HH:mm"), endTime = defaultEndTime.ToString("HH:mm") },
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 2, startTime =defaultStartTime.ToString("HH:mm"), endTime = defaultEndTime.ToString("HH:mm") }, 
			    new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 3, startTime =defaultStartTime.ToString("HH:mm"), endTime = defaultEndTime.ToString("HH:mm") },  
			    new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 4, startTime =defaultStartTime.ToString("HH:mm"), endTime = defaultEndTime.ToString("HH:mm") }, 
			    new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 5, startTime =defaultStartTime.ToString("HH:mm"), endTime = defaultEndTime.ToString("HH:mm") }, 

			    new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 6, startTime = null, endTime = null },
			    new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 7, startTime = null, endTime = null }
			};

			foreach (var item in workHours)
			{
				int newId=await _staffWorkHoursRepository.AddAsync(item);
			}


			return newOrgId;
		}

		public async Task<OrganizationDAO?> GetByIdAsync(int id)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _organizationRepository.GetByIdAsync(id);
			return result;
		}

		public async Task<bool> UpdateAsync(OrganizationDAO dao)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _organizationRepository.UpdateAsync(dao);
			return result;
		}

		public async Task<int> AddOrganizationTravel(OrganizationTravelDAO dao)
		{
			using var con = _unitOfWork.BeginConnection();
			int result = await _organizationTravelRelRepository.AddAsync(dao);
			return result;
		}
	}
}
