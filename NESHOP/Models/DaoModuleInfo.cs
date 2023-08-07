using System.Collections.Generic;

namespace NESHOP.Models
{
    public class DaoModuleInfo
    {
        public string ModuleCode { get; set; }

        public string ModuleDesc { get; set; }

        public List<DaoMenuInfo> MenuInfo { get; set; }

        public string ModuleIcon { get; set; }

        public DaoModuleInfo()
        {
            MenuInfo = new List<DaoMenuInfo>();
        }

    }
}
