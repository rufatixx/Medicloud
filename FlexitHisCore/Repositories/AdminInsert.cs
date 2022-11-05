using System;
using System.Text;
using System.Text.RegularExpressions;
using crypto;
using FlexitHisCore.Models;
using MySql.Data.MySqlClient;

namespace FlexitHisCore.Repositories
{
    public class AdminInsert
    {
        //Communications communications;
        private readonly string ConnectionString;
      
        public AdminInsert(string conString)
        {
            ConnectionString = conString;
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


        



    }
}
