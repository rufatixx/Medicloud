using System;
using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.Plan
{
    public interface IPlanRepository
    {
        PlanDAO GetById(int id);
        UserPlanDAO GetUserPlanByUserId(int userId);
    }
}

