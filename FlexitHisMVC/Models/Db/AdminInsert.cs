using System;
using System.Text;
using System.Text.RegularExpressions;
using crypto;
using FlexitHis_API.Models.Structs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace FlexitHis_API.Models.Db
{
    public class AdminInsert
    {
        //Communications communications;
        private readonly string ConnectionString;
        public IConfiguration Configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public AdminInsert(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            ConnectionString = Configuration.GetSection("ConnectionStrings").GetSection("DefaultConnectionString").Value;
            _hostingEnvironment = hostingEnvironment;
            // communications = new Communications(Configuration, _hostingEnvironment);

        }
        static string sha256(string randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        public long regexPhone(long phone)
        {
            long formattedPhone = 0;


            if (Regex.Match(phone.ToString(), @"[0-9]{12}").Success)
            {
                formattedPhone = Convert.ToInt64(phone.ToString().Substring(3));
            }

            return formattedPhone;
        }
        public bool IsValidPhone(long phone)
        {

            bool isValid = false;



            MySqlConnection connection = new MySqlConnection(ConnectionString);
            try
            {
                //  MailAddress m = new MailAddress(emailaddress);
                connection.Open();

                MySqlCommand com = new MySqlCommand("select * from user where mobile=@phone", connection);
                com.Parameters.AddWithValue("@phone", phone);
                MySqlDataReader reader = com.ExecuteReader();

                bool except = reader.HasRows;
                connection.Close();
                if (except)
                {

                    isValid = true;
                }
                else
                {
                    isValid = false;

                }



            }
            catch (FormatException)
            {
                connection.Close();

            }


            return isValid;

        }
        public string createCode(int length)
        {
            // const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string valid = "1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            while (0 < length--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }


        public ResponseStruct<int> InsertDepartments(long buildingID,string depName, int depTypeID)
        {
            ResponseStruct<int> response = new ResponseStruct<int>();
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO building_to_dep_rel (buildingID,depName,depTypeID) values (@buildingID,@depName,@depTypeID)", connection))

                    {
                        com.Parameters.AddWithValue("@buildingID", buildingID);
                        com.Parameters.AddWithValue("@depName", depName);
                        com.Parameters.AddWithValue("@depTypeID", depTypeID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {



                        response.status = 1; //inserted
                    }
                    else
                    {
                        response.status = 2; // not inserted (Duplicate)
                    }
                   
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4; // not inserted (error)
            }


            return response;
        }
        public ResponseStruct<int> InsertCompanyGroup(int userID,int hospitalID, string cGroupName,int cGroupType)
        {
            ResponseStruct<int> response = new ResponseStruct<int>();
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO company_group (name,type,hospitalID) values (@cGroupName,@groupType,@hospitalID)", connection))

                    {
                        com.Parameters.AddWithValue("@cGroupName", cGroupName);
                        com.Parameters.AddWithValue("@groupType", cGroupType);
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {



                        response.status = 1; //inserted
                    }
                    else
                    {
                        response.status = 2; // not inserted (Duplicate)
                    }
                   
                    connection.Close();
                }


            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4; // not inserted (error)
            }


            return response;
        }
        public ResponseStruct<int> InsertCompany(int userID, int hospitalID, string companyName, int cGroupID)
        {
            ResponseStruct<int> response = new ResponseStruct<int>();
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"Insert INTO company (name,groupID,hospitalID,userID) values (@companyName,@cGroupID,@hospitalID,@userID)", connection))

                    {
                        com.Parameters.AddWithValue("@companyName", companyName);
                        com.Parameters.AddWithValue("@cGroupID", cGroupID);
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        com.Parameters.AddWithValue("@userID", userID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {



                        response.status = 1; //inserted
                    }
                    else
                    {
                        response.status = 2; // not inserted (Duplicate)
                    }
                  
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4; // not inserted (error)
            }


            return response;
        }
        public ResponseStruct<int> UpdateCompany(int userID,int hospitalID,int id, string name, int isActive)
        {
            ResponseStruct<int> response = new ResponseStruct<int>();
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"UPDATE company
SET name = @name,isActive = @isActive WHERE id = @id and hospitalID = @hospitalID;", connection))

                    {
                        com.Parameters.AddWithValue("@id", id);
                        com.Parameters.AddWithValue("@name", name);
                        com.Parameters.AddWithValue("@isActive", isActive);
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        com.Parameters.AddWithValue("@userID", userID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {



                        response.status = 1; //inserted
                    }
                    else
                    {
                        response.status = 2; // not inserted (Duplicate)
                    }
                 
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4; // not inserted (error)
            }


            return response;
        }
        public ResponseStruct<int> UpdateCompanyGroup(int userID,int hospitalID, int id, string name, int isActive)
        {
            ResponseStruct<int> response = new ResponseStruct<int>();
            try
            {
                long lastID = 0;
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"UPDATE company_group
SET name = @name,isActive = @isActive WHERE id = @id and hospitalID = @hospitalID;", connection))

                    {
                        com.Parameters.AddWithValue("@id", id);
                        com.Parameters.AddWithValue("@name", name);
                        com.Parameters.AddWithValue("@isActive", isActive);
                        com.Parameters.AddWithValue("@hospitalID", hospitalID);
                        com.Parameters.AddWithValue("@userID", userID);

                        com.ExecuteNonQuery();
                        lastID = com.LastInsertedId;
                    }



                    if (lastID > 0)
                    {



                        response.status = 1; //inserted
                    }
                    else
                    {
                        response.status = 2; // not inserted (Duplicate)
                    }
                 
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4; // not inserted (error)
            }


            return response;
        }
        public ResponseStruct<int> UpdateDepartments(int id,int gender, long buildingID, int depTypeID,int drIsRequired,int isActive,int isRandevuActive)
        {
            ResponseStruct<int> response = new ResponseStruct<int>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(ConnectionString))
                {

                    connection.Open();





                    using (MySqlCommand com = new MySqlCommand(@"UPDATE building_to_dep_rel
SET buildingID = @buildingID,genderID = @genderID, depTypeID= @depTypeID, DrIsRequired = @DrIsRequired, isActive = @isActive, isRandevuActive =@isRandevuActive
WHERE id = @id;", connection))

                    {
                        com.Parameters.AddWithValue("@id", id);
                        com.Parameters.AddWithValue("@buildingID", buildingID);
                        com.Parameters.AddWithValue("@genderID", gender);
                        com.Parameters.AddWithValue("@depTypeID", depTypeID);
                        com.Parameters.AddWithValue("@DrIsRequired", drIsRequired);
                        com.Parameters.AddWithValue("@isActive", isActive);
                        com.Parameters.AddWithValue("@isRandevuActive", isRandevuActive);

                        com.ExecuteNonQuery();

                    }

                    response.status = 1; //inserted

                   
                    connection.Close();
                }

            }
            catch (Exception ex)
            {
                FlexitHisMVC.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                response.status = 4; // not inserted (error)
            }


            return response;
        }




    }
}
