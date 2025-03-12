using Medicloud.DAL.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medicloud.DAL.Repository.OrganizationRating
{
	public interface IOrganizationRatingRepository
	{
		Task<int>AddAsync(OrganizationRatingDAO organizationRatingDAO);
		Task<int>RemoveAsync(int organizationRatingId);
	}
}
