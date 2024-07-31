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
                organization_id = appointmentDto.OrganizationID,
                service_id = appointmentDto.ServiceId,
                end_date = (appointmentDto.MeetingDate.Date + appointmentDto.Time).AddMinutes(10),
                start_date = appointmentDto.MeetingDate.Date + appointmentDto.Time,
				user_id = appointmentDto.UserId,
				patient_phone = appointmentDto?.PhoneNumber
            };

            bool result = _appointmentRepo.InsertAppointment(appointment);
            return result;
        }

        //public IEnumerable<AppointmentViewModel> GetAllAppointments()
        //{
        //    var result = _appointmentRepo.GetAllAppointment();
        //    return result;
        //}

        public AppointmentViewModel GetAppointmentById(string id)
        {
            var result = _appointmentRepo.GetAppointmentById(id);
            return result;
        }

        public bool DeleteAppointment(string id)
        {
            var result = _appointmentRepo.DeleteAppointment(id);
            return result;
        }

		public bool UpdateAppointment(AddAppointmentDto appointmentDto)
		{
			var appointment = new Appointment()
			{
				id = appointmentDto.Id,
                organization_id = appointmentDto.OrganizationID,
				patient_id = appointmentDto.PatientId,
				service_id = appointmentDto.ServiceId,
				end_date = (appointmentDto.MeetingDate.Date + appointmentDto.Time).AddMinutes(10),
				start_date = appointmentDto.MeetingDate.Date + appointmentDto.Time,
				patient_phone = appointmentDto?.PhoneNumber
			};
			var result = _appointmentRepo.UpdateAppointment(appointment);
			return result;
		}

		public AppointmentPagedResult GetAllAppointments(long organizationID, string searchQuery, int pageNumber)
		{
			var app = _appointmentRepo.GetAllAppointments(organizationID,searchQuery, pageNumber);
			return app;
		}

		public List<AppointmentViewModel> GetAppointmentsByRange(DateTime startDate, DateTime endDate, int userID, int organizationID)
		{
			var res = _appointmentRepo.GetAppointmentsByRange(startDate, endDate, userID, organizationID);
			return res;
		}

		public List<AppointmentViewModel> GetAppointmentByDate(DateTime date, int userID, int organizationID)
		{
			var res = _appointmentRepo.GetAppointmentByDate(date, userID, organizationID);
			return res;
		}
	}
}
