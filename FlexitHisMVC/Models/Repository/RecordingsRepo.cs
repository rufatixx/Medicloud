using System;
using FlexitHisMVC.Models.Domain;
using MySql.Data.MySqlClient;

namespace FlexitHisMVC.Models.Repository
{
	public class RecordingsRepo
	{
        private readonly string ConnectionString;

        public RecordingsRepo(string conString)
        {
            ConnectionString = conString;
        }
  
		public long InsertIntoRecordings(string name,string filePath) {
            long lastID;
            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {

                connection.Open();
                var query = "INSERT INTO records (path, name) VALUES (@path, @name)";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@name", name);
                    command.Parameters.AddWithValue("@path", filePath);
     
                    command.ExecuteNonQuery();
                    lastID = command.LastInsertedId;
                }
            }
            return lastID;
        }
	}
}

