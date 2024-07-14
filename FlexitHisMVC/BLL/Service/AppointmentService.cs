using Medicloud.BLL.Models;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository;

namespace Medicloud.BLL.Service
{
    public class AppointmentService
    {
        private readonly string _connectionString;
        readonly AppointmentRepo _appointmentRepo;

        public AppointmentService(string conString)
        {
            _connectionString = conString;
            _appointmentRepo = new AppointmentRepo(_connectionString);
        }

        public bool AddAppointment(AddAppointmentDto appointmentDto)
        {
            var appointment = new Appointment()
            {
                patient_id = appointmentDto.PatientId,
                service_id = appointmentDto.ServiceId,
                end_date = (appointmentDto.MeetingDate.Date + appointmentDto.Time).AddMinutes(10),
                start_date = appointmentDto.MeetingDate.Date + appointmentDto.Time,
            };

            bool result = _appointmentRepo.InsertAppointment(appointment);
            return result;
        }

        public IEnumerable<AppointmentViewModel> GetAllAppointments()
        {
            var result = _appointmentRepo.GetAllAppointment();
            return result;
        }

        public AppointmentViewModel GetAppointmentById(string id)
        {
            var result = _appointmentRepo.GetAppointmentById(id);
            return result;
        }

    }
}
