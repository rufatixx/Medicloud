using Medicloud.BLL.Models;
using Medicloud.Models;
using MySql.Data.MySqlClient;

namespace Medicloud.DAL.Repository;

public class PaymentRepo
{
	private readonly string ConnectionString;
	private readonly PlanRepository planRepository;
	public PaymentRepo(string connectionString)
	{
		ConnectionString = connectionString;
		planRepository = new PlanRepository(connectionString);
	}


	public void AddTransaction(PaymentViewModel pvm)
	{
		using var con = new MySqlConnection(ConnectionString);
		con.Open();
		var transaction = con.BeginTransaction();

		try
		{

			#region AddTransaction
			using (MySqlCommand cmd =
				new(@"
INSERT INTO transactions (client_rrn, psp_rrn, client_ip_addr,status, month)
VALUES (@client_rrn, @psp_rrn, @client_ip_addr,@status, @month);  SELECT LAST_INSERT_ID();", con, transaction))
			{

				cmd.Parameters.AddWithValue("@client_rrn", pvm.client_rrn);
				cmd.Parameters.AddWithValue("@psp_rrn", pvm.psp_rrn);
				cmd.Parameters.AddWithValue("@client_ip_addr", pvm.client_ip_addr);
				cmd.Parameters.AddWithValue("@status", pvm.status);
				cmd.Parameters.AddWithValue("@month", pvm.month);

				pvm.transaction_id = Convert.ToInt32(cmd.ExecuteScalar());
			}
			#endregion


			#region AddUserPayment
			using (MySqlCommand cmd2 = new (@"INSERT INTO user_payment (user_id, transaction_id, payment_reason_id) 
VALUES (@user_id, @transaction_id, @payment_reason_id); SELECT LAST_INSERT_ID();", con, transaction))
			{
				cmd2.Parameters.AddWithValue("@user_id", pvm.user_id);
				cmd2.Parameters.AddWithValue("@transaction_id", pvm.transaction_id);
				cmd2.Parameters.AddWithValue("@payment_reason_id", pvm.payment_reason_id);
					
				cmd2.ExecuteScalar();
			}
            #endregion



            //				#region UpdateUserSubscription

            //				using (MySqlCommand cmd3 = new(@"UPDATE users 
            //SET subscription_expire_date = @expireDate WHERE id=@id", con, transaction))
            //				{
            //					cmd3.Parameters.AddWithValue("@expireDate", pvm.expireDate);
            //					cmd3.Parameters.AddWithValue("@id", pvm.user_id);

            //					cmd3.ExecuteNonQuery();
            //				}

            //				#endregion
            var isActive = pvm.status == 0;

            #region UpdatePlan
            var plan = planRepository.GetById(pvm.plan_id);

				using (MySqlCommand cmd4 = new (@"
INSERT INTO user_plans (user_id, plan_id, expire_date,isActive) 
VALUES (@user_id, @plan_id, @expire_date,@isActive)", con, transaction))
				{
					cmd4.Parameters.AddWithValue("@user_id", pvm.user_id);
					cmd4.Parameters.AddWithValue("@plan_id", plan.id);
					cmd4.Parameters.AddWithValue("@expire_date", DateTime.Now.AddMonths(plan.duration));
					cmd4.Parameters.AddWithValue("@isActive", isActive);

					cmd4.ExecuteNonQuery();
				}

				#endregion
			
			
			transaction.Commit();

		}
		catch (Exception)
		{
			transaction.Rollback();
			throw;
		}
	}


}
