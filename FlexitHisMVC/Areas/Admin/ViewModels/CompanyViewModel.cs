using Medicloud.Models;

namespace Medicloud.Areas.Admin.ViewModels
{
    public class CompanyViewModel
    {
        public List<CompanyDAO>  Companies { get; set; }
        public List<CompanyGroupDAO> CompanyGroups { get; set; }

    }
}
