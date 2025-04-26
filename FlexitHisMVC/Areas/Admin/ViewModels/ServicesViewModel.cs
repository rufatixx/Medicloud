using Medicloud.Models;

namespace Medicloud.Areas.Admin.ViewModels
{
    public class ServicesViewModel
    {
        public List<ServiceGroup> ServiceGroups { get; set; }
        public List<ServiceObj> Services { get; set; }
        public List<DepartmentDAO> Departments { get; set; }
    }
}
