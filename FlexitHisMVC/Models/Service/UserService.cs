using System;
using FlexitHisMVC.Data;
using FlexitHisMVC.Models.DTO;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models.Login
{
    public class UserService
    {
        private readonly string ConnectionString;

        public UserService(string conString)
        {
            ConnectionString = conString;
        }
        public LogInDTO SignIn(string username, string pass)
        {

            //long formattedPhone = regexPhone(phone);
            LogInDTO status = new LogInDTO();
            status.personal = new Personal();
            status.hospitals = new List<Hospital>();
            status.kassaList = new List<Kassa>();
          
            try
            {
                UserRepo personalDAO = new UserRepo(ConnectionString);
                status.personal = personalDAO.GetUser(username, pass);

                HospitalRepo hospitalDAO = new HospitalRepo(ConnectionString);
                status.hospitals = hospitalDAO.GetHospitalListByUser(status.personal.ID);

                KassaRepo kassaDAO = new KassaRepo(ConnectionString);
                status.kassaList = kassaDAO.GetUserAllowedKassaList(status.personal.ID);


            }
            catch (Exception ex)
            {

                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine($"Exception: {ex.Message}");
                // status.responseString = $"Exception: {ex.Message}";
            }
            return status;




        }
    }
}

