using System;
using System.Collections.Generic;
using System.Dynamic;
using MySql.Data.MySqlClient;

namespace Medicloud.Models
{
    public class PriceGroupCompanyRepository
    {
        private string _connectionString;

        public PriceGroupCompanyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<dynamic> GetPriceGroupDataForCompany(int companyID)
        {
            List<dynamic> results = new List<dynamic>();

            using (MySqlConnection con = new MySqlConnection(_connectionString))
            {
                con.Open();

                string query = @"
                    SELECT a.ID, a.priceGroupID, a.companyID, p.name AS priceGroupName
                    FROM medicloud.price_group_company_rel a
                    JOIN medicloud.price_group p ON a.priceGroupID = p.id
                    WHERE a.companyID = @companyID";

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@companyID", companyID);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dynamic result = new ExpandoObject();
                            result.ID = reader.GetInt32("ID");
                            result.priceGroupID = reader.GetInt32("priceGroupID");
                            result.companyID = reader.GetInt32("companyID");
                            result.priceGroupName = reader.GetString("priceGroupName");
                            results.Add(result);
                        }
                    }
                }
            }

            return results;
        }
    }
}

