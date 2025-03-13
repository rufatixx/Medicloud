using Medicloud.BLL.DTO;
using Medicloud.BLL.Services.FileUpload;
using Medicloud.DAL.DAO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.File;
using Medicloud.DAL.Repository.Organizationn;
using Medicloud.DAL.Repository.OrganizationPlan;
using Medicloud.DAL.Repository.OrganizationTravelRel;
using Medicloud.DAL.Repository.Staff;

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
		private readonly IOrganizationPlanRepository _organizationPlanRepository;
		private readonly IFileRepository _fileRepository;
		public OrganizationService(IUnitOfWork unitOfWork, IOrganizationRepository organizationRepository, IStaffRepository staffRepository, IOrganizationCategoryRelRepository organizationCategoryRelRepository, IOrganizationTravelRelRepository organizationTravelRelRepository, IStaffWorkHoursRepository staffWorkHoursRepository, IOrganizationPlanRepository organizationPlanRepository, IFileRepository fileRepository)
		{
			_unitOfWork = unitOfWork;
			_organizationRepository = organizationRepository;
			_staffRepository = staffRepository;
			_organizationCategoryRelRepository = organizationCategoryRelRepository;
			_organizationTravelRelRepository = organizationTravelRelRepository;
			_staffWorkHoursRepository = staffWorkHoursRepository;
			_organizationPlanRepository = organizationPlanRepository;
			_fileRepository = fileRepository;
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
			TimeSpan defaultStartTime = new TimeOnly(9, 0).ToTimeSpan();
			TimeSpan defaultEndTime = new TimeOnly(18, 0).ToTimeSpan();
			var workHours = new List<StaffWorkHoursDAO>
			{
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 1, startTime =defaultStartTime, endTime = defaultEndTime },
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 2, startTime =defaultStartTime, endTime = defaultEndTime },
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 3, startTime =defaultStartTime, endTime = defaultEndTime },
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 4, startTime =defaultStartTime, endTime = defaultEndTime },
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 5, startTime =defaultStartTime, endTime = defaultEndTime },

				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 6, startTime = null, endTime = null },
				new StaffWorkHoursDAO { staffId = staffId, dayOfWeek = 7, startTime = null, endTime = null }
			};

			foreach (var item in workHours)
			{
				int newId = await _staffWorkHoursRepository.AddAsync(item);
			}


			return newOrgId;
		}

		public async Task<OrganizationDAO?> GetByIdAsync(int id)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _organizationRepository.GetByIdAsync(id);
			return result;
		}

		public async Task<bool> UpdateAsync(UpdateOrganizationDTO dto)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _organizationRepository.UpdateAsync(new()
			{
				id=dto.Id,
				name = dto.Name,
				workPlaceType=dto.WorkPlaceType,
				latitude=dto.Latitude,
				longitude=dto.Longitude,
				address=dto.Address,
				teamSizeId=dto.TeamSizeId,
				description=dto.Description,
				fbLink=dto.FbLink,
				website=dto.WebLink,
				onlineShopLink=dto.OnlineShopLink,
				insLink=dto.ILink,

			});
			return result;
		}

		public async Task<int> AddOrganizationTravel(OrganizationTravelDAO dao)
		{
			using var con = _unitOfWork.BeginConnection();
			int result = await _organizationTravelRelRepository.AddAsync(dao);
			return result;
		}


		public async Task<OrganizationTravelDAO?> GetOrganizationTravel(int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			var result = await _organizationTravelRelRepository.GetByOrganizationIdAsync(organizationId);
			return result;
		}

		public async Task<bool> UpdateOrganizationTravel(OrganizationTravelDAO dao)
		{
			using var con = _unitOfWork.BeginConnection();
			bool result = await _organizationTravelRelRepository.UpdateAsync(dao);
			return result;
		}

		public async Task<int> UpdateOrganizationCategories(int organizationId, List<int> selectedCategories)
		{
			using var con = _unitOfWork.BeginConnection();
			var categories = await _organizationCategoryRelRepository.GetByOrganizationId(organizationId);

			var currentCategoryIds = categories.Select(c => c.SecondModelId).ToList();


			var addedCategories = selectedCategories.Except(currentCategoryIds).ToList();


			var removedCategories = currentCategoryIds.Except(selectedCategories).ToList();


			foreach (var categoryId in addedCategories)
			{

				await _organizationCategoryRelRepository.AddAsync(organizationId, categoryId);
			}

			foreach (var categoryId in removedCategories)
			{

				var relationToRemove = categories.FirstOrDefault(c => c.SecondModelId == categoryId);
				if (relationToRemove != null)
				{

					await _organizationCategoryRelRepository.RemoveAsync(relationToRemove.id);
				}
			}

			return addedCategories.Count + removedCategories.Count;
		}

		public async Task<int> AddOrganizationPlanAsync(OrganizationPlanDAO dao)
		{
			using var con = _unitOfWork.BeginConnection();
			int newPlanId = await _organizationPlanRepository.AddAsync(dao);
			if (newPlanId > 0)
			{
				await _organizationRepository.UpdateAsync(new()
				{
					id = dao.organizationId,
					isRegistered = true,
				});
			}
			return newPlanId;
		}

		public async Task<List<OrganizationPlanDAO>> GetPlansByOrganizationId(int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			return await _organizationPlanRepository.GetPlansByOrganizationId(organizationId);
		}

		public async Task UpdateLogo(FileDTO file, int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			int newFileId = 0;
			if (file != null)
			{
				newFileId = await _fileRepository.AddFileAsync(new()
				{
					fileName = file.fileName,
					filePath = file.filePath,
				});
			}
			await _organizationRepository.UpdateLogoId(organizationId, newFileId);

		}

		public async Task UpdateCover(FileDTO file, int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			int newFileId = 0;
			if (file != null)
			{
				newFileId = await _fileRepository.AddFileAsync(new()
				{
					fileName = file.fileName,
					filePath = file.filePath,
				});
			}
			await _organizationRepository.UpdateCoverId(organizationId, newFileId);

		}

		public async Task UpdateCoverId(int fileId, int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			await _organizationRepository.UpdateCoverId(organizationId, fileId);
		}
	}
}
