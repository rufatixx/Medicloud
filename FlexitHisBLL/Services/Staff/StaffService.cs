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

        public async Task<List<StaffWorkHoursDTO>> GetWorkHours(int staffId)
        {
            using var con = _unitOfWork.BeginConnection();
            var workhours = await _workHoursRepository.GetStaffWorkHours(staffId);
            var result = new List<StaffWorkHoursDTO>();
            foreach (var item in workhours)
            {
                //Console.WriteLine(item.id.ToString());
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

        public async Task<bool> UpdateStaffWorkHours(UpdateStaffWorkHourDTO dto)
        {
            using var con = _unitOfWork.BeginConnection();
            bool result = false;
            if (dto == null) return false;
            if (dto.ClosedDays?.Count > 0)
            {
                foreach (var dayId in dto.ClosedDays)
                {
                    result =await _workHoursRepository.UpdateAsync(new()
                    {
                        id=dayId,
                        startTime=null,
                        endTime=null,
                    });
					await _workHoursRepository.RemoveAllBreaksByWorkHourIdAsync(dayId);

                }
            }
            if (dto.SelectedDays?.Count > 0)
            {
                foreach (var dayId in dto.SelectedDays)
                {
                    result=await _workHoursRepository.UpdateAsync(new()
                    {
                        id=dayId,
                        startTime=dto.startTime,
                        endTime=dto.endTime,
                    });
                    var dayExistBreaks = await _workHoursRepository.GetStaffBreaksWithWorkHourId(dayId);

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
                                    staffWorkHourId= dayId,
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
                            await _workHoursRepository.AddBreakAsync(new ()
                            {
                                staffWorkHourId = dayId,
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
                    var day = await _workHoursRepository.GetStaffWorkHourById(dayId);
                    if (day.startTime== null|| day.endTime ==null)
                    {
                        result =await _workHoursRepository.UpdateAsync(new()
                        {
                            id=dayId,
                            startTime= new TimeOnly(9, 0).ToTimeSpan(),
                            endTime= new TimeOnly(18, 0).ToTimeSpan()

                        });
                    }

                }
            }
            return true;
        }
    }
}
