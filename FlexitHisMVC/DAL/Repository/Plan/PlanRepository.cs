using Dapper;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Infrastructure.Abstract;

namespace Medicloud.DAL.Repository.Plan
{
    public class PlanRepository:IPlanRepository
    {
        private readonly IUnitOfWork _unitOfWork;

        public PlanRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public PlanDAO GetById(int id)
        {
            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string sql = @"SELECT * FROM plans WHERE id = @id";

                return con.QueryFirstOrDefault<PlanDAO>(sql, new { id });
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        public UserPlanDAO GetUserPlanByUserId(int userId)
        {
            try
            {
                _unitOfWork.BeginConnection();
                var con = _unitOfWork.GetConnection();

                string sql = @"SELECT * FROM user_plans 
                               WHERE user_id = @userId AND isActive = 1";

                return con.QueryFirstOrDefault<UserPlanDAO>(sql, new { userId });
            }
            catch (Exception ex)
            {
                Medicloud.StandardMessages.CallSerilog(ex);
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
