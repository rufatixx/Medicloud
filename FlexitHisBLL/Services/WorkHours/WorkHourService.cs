using Medicloud.BLL.DTO;
using Medicloud.DAL.Infrastructure.UnitOfWork;
using Medicloud.DAL.Repository.WorkHour;

namespace Medicloud.BLL.Services.WorkHours
{
	public class WorkHourService:IWorkHourService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWorkHoursRepository _workHoursRepository;

		public WorkHourService(IUnitOfWork unitOfWork, IWorkHoursRepository workHoursRepository)
		{
			_unitOfWork = unitOfWork;
			_workHoursRepository = workHoursRepository;
		}

		public async Task<List<WorkHourDTO>> GetStaffWorkHours(int staffId)
		{
			using var con = _unitOfWork.BeginConnection();
			var workhours = await _workHoursRepository.GetStaffWorkHours(staffId);
			var result = new List<WorkHourDTO>();
			foreach (var item in workhours)
			{
				var dto = new WorkHourDTO()
				{
					id = item.id,
					startTime = item.startTime,
					endTime = item.endTime,
					dayOfWeek = item.dayOfWeek,
				};
				var breaks = await _workHoursRepository.GetBreaksWithWorkHourId(item.id);
				if (breaks != null)
				{
					dto.Breaks = new();
					foreach (var breakItem in breaks)
					{
						dto.Breaks.Add(new()
						{
							id = breakItem.id,
							start = breakItem.startTime,
							end = breakItem.endTime,
						});
					}
				}
				result.Add(dto);
			}
			return result;
		}

		public async Task<List<WorkHourDTO>> GetOrganizationWorkHours(int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			var workhours = await _workHoursRepository.GetOrganizationWorkHours(organizationId);
			var result = new List<WorkHourDTO>();
			foreach (var item in workhours)
			{
				var dto = new WorkHourDTO()
				{
					id = item.id,
					startTime = item.startTime,
					endTime = item.endTime,
					dayOfWeek = item.dayOfWeek,
				};
				var breaks = await _workHoursRepository.GetBreaksWithWorkHourId(item.id);
				if (breaks != null)
				{
					dto.Breaks = new();
					foreach (var breakItem in breaks)
					{
						dto.Breaks.Add(new()
						{
							id = breakItem.id,
							start = breakItem.startTime,
							end = breakItem.endTime,
						});
					}
				}
				result.Add(dto);
			}
			return result;
		}
		public async Task<bool> UpdateWorkHours(UpdateWorkHourDTO dto)
		{
			using var con = _unitOfWork.BeginConnection();
			bool result = false;
			if (dto == null) return false;
			if (dto.ClosedDays?.Count > 0)
			{
				foreach (var dayId in dto.ClosedDays)
				{
					result = await _workHoursRepository.UpdateAsync(new()
					{
						id = dayId,
						startTime = null,
						endTime = null,
					});
					await _workHoursRepository.RemoveAllBreaksByWorkHourIdAsync(dayId);

				}
			}
			if (dto.SelectedDays?.Count > 0)
			{
				foreach (var dayId in dto.SelectedDays)
				{
					result = await _workHoursRepository.UpdateAsync(new()
					{
						id = dayId,
						startTime = dto.startTime,
						endTime = dto.endTime,
					});
					var dayExistBreaks = await _workHoursRepository.GetBreaksWithWorkHourId(dayId);

					if (dayExistBreaks != null)
					{

						foreach (var existBreak in dayExistBreaks)
						{

							var match = dto.Breaks.Any(b => b.start == existBreak.startTime && b.end == existBreak.endTime);
							if (!match)
							{

								await _workHoursRepository.RemoveBreakAsync(existBreak.id);
							}
						}


						foreach (var item in dto.Breaks)
						{
							var exists = dayExistBreaks.Any(b => b.startTime == item.start && b.endTime == item.end);
							if (!exists)
							{

								await _workHoursRepository.AddBreakAsync(new()
								{
									workHourId = dayId,
									startTime = item.start,
									endTime = item.end
								});
							}
						}
					}
					else
					{
						foreach (var item in dto.Breaks)
						{
							await _workHoursRepository.AddBreakAsync(new()
							{
								workHourId = dayId,
								startTime = item.start,
								endTime = item.end
							});
						}
					}

				}
			}
			if (dto.OpenedDays?.Count > 0)
			{
				foreach (var dayId in dto.OpenedDays)
				{
					if (dto.SelectedDays != null && dto.SelectedDays.Contains(dayId)) continue;
					var day = await _workHoursRepository.GetWorkHourById(dayId);
					if (day.startTime == null || day.endTime == null)
					{
						result = await _workHoursRepository.UpdateAsync(new()
						{
							id = dayId,
							startTime = new TimeOnly(9, 0).ToTimeSpan(),
							endTime = new TimeOnly(18, 0).ToTimeSpan()

						});
					}

				}
			}
			return true;
		}
	}
}
