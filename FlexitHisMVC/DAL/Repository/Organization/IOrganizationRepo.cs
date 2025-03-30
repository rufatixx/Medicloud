using System;
using Medicloud.DAL.Entities;

namespace Medicloud.DAL.Repository.Organization
{
    public interface IOrganizationRepo
    {
        long InsertOrganization(string organizationName);
        List<OrganizationDAO> GetOrganizationListWhereUserIsManager(int userID);
        List<OrganizationDAO> GetOrganizationListByUser(int userID);
        List<OrganizationDAO> GetOrganizationList();
        long InsertOrganizationToUser(long userID, long organizationID);
        int RemoveOrganizationFromUser(int userID, int organizationID);
    }
}

