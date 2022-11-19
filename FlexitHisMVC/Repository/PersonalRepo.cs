using System;
using System.Net.NetworkInformation;
using FlexitHisMVC.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Data
{
    public class PersonalRepo
    {
        private readonly string ConnectionString;

        public PersonalRepo(string conString)
        {
            ConnectionString = conString;
        }
        public List<Personal> GetPersonalList()
        {
            List<Personal> personalList = new List<Personal>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT *,(select name from speciality where a.specialityID = id )as specialityName FROM users a ", connection))
                {

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {

                            Personal personal = new Personal();
                            personal.ID = Convert.ToInt32(reader["id"]);
                            personal.depID = Convert.ToInt32(reader["departmentID"]);
                            personal.name = reader["name"].ToString();
                            personal.surname = reader["surname"].ToString();
                            personal.father = reader["father"].ToString();
                            personal.mobile = reader["mobile"].ToString();
                            personal.email = reader["email"].ToString();

                            personal.bDate = Convert.ToDateTime(reader["bDate"]).Date;
                            personal.speciality = reader["specialityName"].ToString();
                            personal.isActive = Convert.ToBoolean(reader["isActive"]);
                            personal.isUser = Convert.ToBoolean(reader["isUser"]);
                            personal.isDr = Convert.ToBoolean(reader["isDr"]);

                            personalList.Add(personal);


                        }



                    }

                }
                connection.Close();
            }
            return personalList;
        }

        public List<Personal> GetRefererList()
        {
            List<Personal> refererList = new List<Personal>();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("SELECT * FROM users where referral = 1;", connection))
                {

                    MySqlDataReader reader = com.ExecuteReader();
                    if (reader.HasRows)
                    {


                        while (reader.Read())
                        {

                            Personal referralPersonal = new Personal();
                            referralPersonal.ID = Convert.ToInt32(reader["id"]);
                            referralPersonal.name = reader["name"].ToString();
                            referralPersonal.surname = reader["surname"].ToString();
                            referralPersonal.father = reader["father"].ToString();
                            refererList.Add(referralPersonal);


                        }



                    }

                }

                connection.Close();
            }
            return refererList;
        }

        public Personal GetUser(string username, string pass)
        {
            Personal personal = new Personal();

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                using (MySqlCommand com = new MySqlCommand("select * from users where pwd=SHA2(@pass,256) and username = @username and isActive=1 and isUser=1", connection))
                {

                    com.Parameters.AddWithValue("@pass", pass);
                    com.Parameters.AddWithValue("@username", username);
                    using (MySqlDataReader reader = com.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {


                            while (reader.Read())
                            {
                                personal.ID = Convert.ToInt32(reader["id"]);
                                personal.isActive = Convert.ToBoolean(reader["isActive"]);
                                personal.isUser = Convert.ToBoolean(reader["isUser"]);
                                personal.name = reader["name"].ToString();
                                personal.surname = reader["surname"].ToString();
                            }

                            connection.Close();





                        }

                    }

                }
                connection.Close();
            }
            return personal;
        }
        public int InsertPersonal(string name, string surname, string father,string passportSerialNum,string fin, string phone, string email, string bDate, string username, string pwd, int isUser)
        {
            int lastID = 0;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {




                    connection.Open();
                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO users (name,surname,father,mobile,email,bdate,username,pwd,isUser)
values (@name,@surname,@father,@mobile,@email,@bDate,@username,@pwd,@isUser)", connection))
                    {
                        com.Parameters.AddWithValue("@name", name);
                        com.Parameters.AddWithValue("@surname", surname);
                        com.Parameters.AddWithValue("@father", father);
                        com.Parameters.AddWithValue("@passportSerialNum", passportSerialNum);
                        com.Parameters.AddWithValue("@fin", fin);
                        com.Parameters.AddWithValue("@mobile", phone);
                        com.Parameters.AddWithValue("@email", email);
                        com.Parameters.AddWithValue("@bDate", bDate);
                        com.Parameters.AddWithValue("@username", username);
                        com.Parameters.AddWithValue("@pwd", pwd);
                        com.Parameters.AddWithValue("@isUser", isUser);

                        lastID = com.ExecuteNonQuery();


                    }
                    connection.Close();



                }


            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);


            }
            return lastID;
        }

        //public bool UpdatePersonal(int userID, string name, string surname, string father, string phone, string email, string bDate, string pwd)
        //{
        //    List<Personal> personalList = new List<Personal>();
        //    try
        //    {
        //        using (MySqlConnection connection = new MySqlConnection(ConnectionString))
        //        {




        //            connection.Open();
        //            using (MySqlCommand com = new MySqlCommand("SELECT *,(select name from speciality where a.specialityID = id )as specialityName FROM personal a", connection))
        //            {

        //                MySqlDataReader reader = com.ExecuteReader();
        //                if (reader.HasRows)
        //                {


        //                    while (reader.Read())
        //                    {

        //                        PersonalStruct personal = new PersonalStruct();
        //                        personal.ID = Convert.ToInt32(reader["id"]);
        //                        //personal.depID = Convert.ToInt32(reader["departmentID"]);
        //                        personal.name = reader["name"].ToString();
        //                        personal.surname = reader["surname"].ToString();
        //                        personal.father = reader["father"].ToString();
        //                        personal.speciality = reader["specialityName"].ToString();
        //                        personal.isActive = Convert.ToBoolean(reader["isActive"]);
        //                        personal.isUser = Convert.ToBoolean(reader["isUser"]);

        //                        personalList.Add(personal);
        //                    }



        //                }

        //            }
        //            connection.Close();



        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        Console.WriteLine(ex.Message);


        //    }
        //    return personalList;
        //}

    }
}

