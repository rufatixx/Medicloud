using Medicloud.BLL.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository;

public class PaymentRepo
{
	private readonly string ConnectionString;

	public PaymentRepo(string connectionString)
	{
		ConnectionString = connectionString;
	}


	public void AddTransaction(PaymentViewModel pvm)
	{
		using (MySqlConnection con = new MySqlConnection(ConnectionString))
		{
			con.Open();
			MySqlTransaction transaction = con.BeginTransaction();
			
			try
			{
				using (MySqlCommand cmd = new("INSERT INTO transactions (client_rrn, psp_rrn, client_ip_addr) VALUES (@client_rrn, @psp_rrn, @client_ip_addr);  SELECT LAST_INSERT_ID();", con, transaction))
				{

					cmd.Parameters.AddWithValue("@client_rrn", pvm.client_rrn);
					cmd.Parameters.AddWithValue("@psp_rrn", pvm.psp_rrn);
					cmd.Parameters.AddWithValue("@client_ip_addr", pvm.client_ip_addr);

					pvm.transaction_id = Convert.ToInt32(cmd.ExecuteScalar());
				}


				using (MySqlCommand cmd2 = new ("INSERT INTO user_payment (user_id, transaction_id, payment_reason_id) VALUES (@user_id, @transaction_id, @payment_reason_id); SELECT LAST_INSERT_ID();", con, transaction))
				{
					cmd2.Parameters.AddWithValue("@user_id", pvm.user_id);
					cmd2.Parameters.AddWithValue("@transaction_id", pvm.transaction_id);
					cmd2.Parameters.AddWithValue("@payment_reason_id", pvm.payment_reason_id);
					
					cmd2.ExecuteScalar();
				}

				transaction.Commit();

			}
			catch (Exception)
			{

				throw;
			}
		}
	}


}
