using Medicloud.DAL.Entities;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository;

public class PlanRepository
{
	private readonly string ConnectionString;

	public PlanRepository(string connectionString)
	{
		ConnectionString = connectionString;
	}


	public Plan GetById(int id)
	{
		using var con = new MySqlConnection(ConnectionString);
		con.Open();

		Plan plan = new Plan();

		try
		{
			using (MySqlCommand cmd = new("SELECT * FROM plans WHERE id=@id", con))
			{
				cmd.Parameters.AddWithValue("@id", id);
				var reader = cmd.ExecuteReader();
				if (reader.Read())
				{
					plan = new Plan
					{
						id = Convert.ToInt32(reader["id"]),
						duration = Convert.ToInt32(reader["id"]),
						name = reader["name"].ToString(),
						price = reader.GetDecimal("price")
					};
				}
				con.Close();
			}
		}
		catch (Exception ex)
		{
			Medicloud.StandardMessages.CallSerilog(ex);
			Console.WriteLine(ex.Message);
		}

		return plan;

	}

	public UserPlan GetUserPlanByUserId(int userId)
	{
		using var con = new MySqlConnection(ConnectionString);
		con.Open();

		UserPlan plan = new UserPlan();

		try
		{
			using (MySqlCommand cmd = new("SELECT * FROM user_plans WHERE user_id=@userId", con))
			{
				cmd.Parameters.AddWithValue("@userId", userId);
				var reader = cmd.ExecuteReader();
				if (reader.Read())
				{
					plan = new UserPlan
					{
						id = Convert.ToInt32(reader["id"]),
						plan_id = Convert.ToInt32(reader["plan_id"]),
						user_id = Convert.ToInt32(reader["user_id"]),
						cDate = reader["cDate"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["cDate"]),
						expire_date = reader["expire_date"] == DBNull.Value ? default(DateTime) : Convert.ToDateTime(reader["expire_date"])
					};
				}
				con.Close();
			}
		}
		catch (Exception ex)
		{
			Medicloud.StandardMessages.CallSerilog(ex);
			Console.WriteLine(ex.Message);
		}

		return plan;
	}
}
