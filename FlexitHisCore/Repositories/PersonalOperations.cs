using System;
using FlexitHisCore.Models;
using FlexitHisCore.Models.Admin;
using MySql.Data.MySqlClient;

namespace FlexitHisCore
{
    public class PersonalOperations
    {
        private readonly string ConnectionString;
       
        public PersonalOperations(string conString)
        {

            ConnectionString = conString;
            

        }
        public List<PersonalDTO> GetPersonals(int userID)
        {
            List<PersonalStruct> personalList = new List<PersonalStruct>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {


       
                 
                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand("SELECT *,(select name from speciality where a.specialityID = id )as specialityName FROM personal a", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PersonalStruct personal = new PersonalStruct();
                                personal.ID = Convert.ToInt32(reader["id"]);
                                //personal.depID = Convert.ToInt32(reader["departmentID"]);
                                personal.name = reader["name"].ToString();
                                personal.surname = reader["surname"].ToString();
                                personal.father = reader["father"].ToString();
                                personal.speciality = reader["specialityName"].ToString();
                                personal.isActive = Convert.ToBoolean(reader["isActive"]);
                                personal.isUser = Convert.ToBoolean(reader["isUser"]);

                                personalList.Add(personal);
                            }



                        }

                    }
                    connection.Close();
     


                }


            }
            catch (Exception ex)
            {
                FlexitHisCore.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
             

            }
            return personalList ;
        }
        public List<PersonalStruct> UpdatePersonal(int userID,string name, string surname, string father, string phone,string email, string bDate,string pwd )
        {
            List<PersonalStruct> personalList = new List<PersonalStruct>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {




                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand("SELECT *,(select name from speciality where a.specialityID = id )as specialityName FROM personal a", connection))
                    {

                        MySqlDataReader reader = com.ExecuteReader();
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {

                                PersonalStruct personal = new PersonalStruct();
                                personal.ID = Convert.ToInt32(reader["id"]);
                                //personal.depID = Convert.ToInt32(reader["departmentID"]);
                                personal.name = reader["name"].ToString();
                                personal.surname = reader["surname"].ToString();
                                personal.father = reader["father"].ToString();
                                personal.speciality = reader["specialityName"].ToString();
                                personal.isActive = Convert.ToBoolean(reader["isActive"]);
                                personal.isUser = Convert.ToBoolean(reader["isUser"]);

                                personalList.Add(personal);
                            }



                        }

                    }
                    connection.Close();



                }


            }
            catch (Exception ex)
            {
                FlexitHisCore.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);


            }
            return personalList;
        }
    }
}

