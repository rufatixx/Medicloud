using System;
using Medicloud.Models;
using Medicloud.Models.DTO;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository
{
    public class UserPlanRepo
    {

        private readonly string _connectionString;

        public UserPlanRepo(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void AddUserPlan(int userId, int planId, int duration, bool isActive)
        {
            using (var con = new MySqlConnection(_connectionString))
            {
                con.Open();
                using (var transaction = con.BeginTransaction())
                {
                    try
                    {
                        using (var cmd = new MySqlCommand(@"
INSERT INTO user_plans (user_id, plan_id, expire_date, isActive) 
VALUES (@user_id, @plan_id, @expire_date, @isActive)", con, transaction))
                        {
                            cmd.Parameters.AddWithValue("@user_id", userId);
                            cmd.Parameters.AddWithValue("@plan_id", planId);
                            cmd.Parameters.AddWithValue("@expire_date", DateTime.Now.AddMonths(duration));
                            cmd.Parameters.AddWithValue("@isActive", isActive);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception("Failed to add user plan", ex);
                    }
                }
            }
        }
    }
}

