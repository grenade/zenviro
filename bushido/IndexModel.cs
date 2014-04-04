using System.Collections;
using System.Collections.Generic;

namespace Zenviro.Bushido
{
    public class IndexModel
    {
        public List<ApplicationGroupModel> ApplicationGroups { get; set; }
        public List<ApplicationEnvironmentModel> ApplicationEnvironments { get; set; }
        public List<HostModel> Hosts { get; set; }
        public List<SearchPathModel> SearchPaths { get; set; }
        public List<AppModel> Apps { get; set; }
    }
}