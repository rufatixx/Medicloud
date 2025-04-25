using System;
using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.UserPlan
{
    public class UserPlanRepo:IUserPlanRepo
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserPlanRepo(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void AddUserPlan(int userId, int planId, int duration, bool isActive)
        {
            _unitOfWork.BeginConnection();
            var con = _unitOfWork.GetConnection();
            var transaction = _unitOfWork.BeginTransaction();

            try
            {
                var query = @"
                    INSERT INTO user_plans (user_id, plan_id, expire_date, isActive) 
                    VALUES (@user_id, @plan_id, @expire_date, @isActive);";

                con.Execute(query, new
                {
                    user_id = userId,
                    plan_id = planId,
                    expire_date = DateTime.Now.AddMonths(duration),
                    isActive
				}, transaction);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Failed to add user plan", ex);
            }
        }

		public async Task<UserPlanDAO> GetPlansByUserId(int userId)
		{
			using var con=_unitOfWork.BeginConnection();
			var query = "SELECT * FROM user_plans WHERE user_id =@UserId AND isActive=1";

			var result = await con.QuerySingleOrDefaultAsync<UserPlanDAO>(query, new { UserId = userId });
			return result;
		}
	}
}
