using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;
using Medicloud.DAL.Repository.WorkHour;

namespace Medicloud.BLL.Services.WorkHour
{
	public class WorkHourService:IWorkHourService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWorkHourRepository _workHourRepository;

		public WorkHourService(IUnitOfWork unitOfWork, IWorkHourRepository workHourRepository)
		{
			_unitOfWork = unitOfWork;
			_workHourRepository = workHourRepository;
		}

		public async Task<int> AddOrganizationUserWorkHourAsync(int organizationId,int userId)
		{
			using var con = _unitOfWork.BeginConnection();


			TimeSpan defaultStartTime = new TimeOnly(9, 0).ToTimeSpan();
			TimeSpan defaultEndTime = new TimeOnly(18, 0).ToTimeSpan();
			var workHours = new List<WorkHourDAO>
			{
				new WorkHourDAO { dayOfWeek = 1, startTime =defaultStartTime, endTime = defaultEndTime },
				new WorkHourDAO { dayOfWeek = 2, startTime =defaultStartTime, endTime = defaultEndTime },
				new WorkHourDAO { dayOfWeek = 3, startTime =defaultStartTime, endTime = defaultEndTime },
				new WorkHourDAO { dayOfWeek = 4, startTime =defaultStartTime, endTime = defaultEndTime },
				new WorkHourDAO { dayOfWeek = 5, startTime =defaultStartTime, endTime = defaultEndTime },
				new WorkHourDAO { dayOfWeek = 6, startTime = null, endTime = null },
				new WorkHourDAO { dayOfWeek = 7, startTime = null, endTime = null }
			};
			int newWorkHourId = 0;
			foreach (var item in workHours)
			{
				item.userId = userId;
				item.organizationId = organizationId;
				newWorkHourId = await _workHourRepository.AddOrganizationUserWorkHourAsync(item);
			}

			return newWorkHourId;
		}

		public Task<bool> UpdateAsync(WorkHourDAO dao)
		{
			throw new NotImplementedException();
		}

		public async Task<List<WorkHourDTO>> GetOrganizationUserWorkHours(int userId, int organizationId)
		{
			using var con = _unitOfWork.BeginConnection();
			//var result=await _workHourRepository.GetOrganizationUserWorkHours(userId, organizationId);
			//if(result != null)
			//{
			//	foreach (var item in result)
			//	{
			//		item.Breaks = new();
			//	}

			//}

			var workhours =  await _workHourRepository.GetOrganizationUserWorkHours(userId, organizationId);
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
				var breaks = await _workHourRepository.GetBreaksWithWorkHourId(item.id);
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

		public Task<int> AddBreakAsync(BreakDAO dao)
		{
			throw new NotImplementedException();
		}

		public Task<bool> RemoveBreakAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> RemoveAllBreaksByWorkHourIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		public Task<bool> UpdateBreakAsync(BreakDAO dao)
		{
			throw new NotImplementedException();
		}

		public Task<List<BreakDAO>> GetBreaksWithWorkHourId(int workHourId)
		{
			throw new NotImplementedException();
		}

		public Task<WorkHourDAO> GetWorkHourById(int id)
		{
			throw new NotImplementedException();
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
					result = await _workHourRepository.UpdateAsync(new()
					{
						id = dayId,
						startTime = null,
						endTime = null,
					});
					await _workHourRepository.RemoveAllBreaksByWorkHourIdAsync(dayId);

				}
			}
			if (dto.SelectedDays?.Count > 0)
			{
				foreach (var dayId in dto.SelectedDays)
				{
					result = await _workHourRepository.UpdateAsync(new()
					{
						id = dayId,
						startTime = dto.startTime,
						endTime = dto.endTime,
					});
					var dayExistBreaks = await _workHourRepository.GetBreaksWithWorkHourId(dayId);

					if (dayExistBreaks != null)
					{

						foreach (var existBreak in dayExistBreaks)
						{

							var match = dto.Breaks.Any(b => b.start == existBreak.startTime && b.end == existBreak.endTime);
							if (!match)
							{

								await _workHourRepository.RemoveBreakAsync(existBreak.id);
							}
						}


						foreach (var item in dto.Breaks)
						{
							var exists = dayExistBreaks.Any(b => b.startTime == item.start && b.endTime == item.end);
							if (!exists)
							{

								await _workHourRepository.AddBreakAsync(new()
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
							await _workHourRepository.AddBreakAsync(new()
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
					var day = await _workHourRepository.GetWorkHourById(dayId);
					if (day.startTime == null || day.endTime == null)
					{
						result = await _workHourRepository.UpdateAsync(new()
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
