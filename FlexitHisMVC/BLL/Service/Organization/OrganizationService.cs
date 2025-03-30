using Medicloud.BLL.Service.Organization;
using Medicloud.DAL.Entities;
using Medicloud.DAL.Repository;
using Medicloud.DAL.Repository.Organization;
using Medicloud.Models;

namespace Medicloud.BLL.Service.Organization
{
    public class OrganizationService:IOrganizationService
    {
      
        readonly IOrganizationRepo _organizationRepo;

        public OrganizationService(IOrganizationRepo organizationRepo)
        {

            _organizationRepo = organizationRepo;

        }

        public long AddOrganization(string organizationName)
        {

            if (string.IsNullOrEmpty(organizationName))
            {
                throw new ArgumentException("Organization name must not be empty", nameof(organizationName));
            }

            return _organizationRepo.InsertOrganization(organizationName);


        }
        public long AddOrganizationToNewUser(long userID, string organizationName)
        {
            long orgID = AddOrganization(organizationName);
            var orgToUserRelID = _organizationRepo.InsertOrganizationToUser(userID, orgID);

            if (orgToUserRelID > 0)
            {
                return orgID;
            }

            return 0;

        }

        public long AddOrganizationToUser(long userID, long orgID)
        {

            return _organizationRepo.InsertOrganizationToUser(userID, orgID);


        }

        // Retrieves all organizations
        public List<OrganizationDAO> GetAllOrganizations()
        {
            return _organizationRepo.GetOrganizationList();
        }

        public List<OrganizationDAO> GetOrganizationListWhereUserIsManager(int userID)
        {
            return _organizationRepo.GetOrganizationListWhereUserIsManager(userID);
        }
        // Retrieves organizations by user ID
        public List<OrganizationDAO> GetOrganizationsByUser(int userID)
        {
        //    if (userID <= 0)
        //    {
        //        throw new ArgumentException("User ID must be positive", nameof(userID));
        //    }

            return _organizationRepo.GetOrganizationListByUser(userID);
        }

        // Links an organization to a user if not already linked
        public long LinkOrganizationToUser(long userID, long organizationID)
        {
            if (userID <= 0 || organizationID <= 0)
            {
                throw new ArgumentException("User ID and Organization ID must be positive");
            }

            return _organizationRepo.InsertOrganizationToUser(userID, organizationID);
        }

        // Removes an organization from a user
        public int UnlinkOrganizationFromUser(int userID, int organizationID)
        {
            return userID <= 0 || organizationID <= 0
                ? throw new ArgumentException("User ID and Organization ID must be positive")
                : _organizationRepo.RemoveOrganizationFromUser(userID, organizationID);
        }

    }


}




