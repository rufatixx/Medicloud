using System;
using Medicloud.DAL.Entities;

namespace Medicloud.BLL.Service.Organization
{
    public interface IOrganizationService
    {
        long AddOrganization(string organizationName);
        long AddOrganizationToNewUser(long userID, string organizationName);
        long AddOrganizationToUser(long userID, long orgID);
        List<OrganizationDAO> GetAllOrganizations();
        List<OrganizationDAO> GetOrganizationListWhereUserIsManager(int userID);
        List<OrganizationDAO> GetOrganizationsByUser(int userID);
        long LinkOrganizationToUser(long userID, long organizationID);
        int UnlinkOrganizationFromUser(int userID, int organizationID);
    }
}

