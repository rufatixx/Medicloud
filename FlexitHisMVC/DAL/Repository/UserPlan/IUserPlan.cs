using System;
namespace Medicloud.DAL.Repository.UserPlan
{
    public interface IUserPlanRepo
    {
        void AddUserPlan(int userId, int planId, int duration, bool isActive);
    }
}

