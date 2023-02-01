using System;
using System.Collections.Generic;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{

    public class RequestTypeRepo
    {
        private readonly string ConnectionString;

        public RequestTypeRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<RequestType> GetRequestType() {
            try
            {

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
            }
            List <RequestType> requestTypeList = new List<RequestType>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();

                using (MySqlCommand com = new MySqlCommand("SELECT * FROM request_type;", connection))
                {

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {
                            RequestType requestType = new RequestType();

                            requestType.ID = Convert.ToInt32(reader["id"]);
                            requestType.name = reader["name"].ToString();

                            requestTypeList.Add(requestType);

                        }
                    }

                }
                connection.Close();
            }
            return requestTypeList;
        }
    }
}

