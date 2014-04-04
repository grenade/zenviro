using System.Collections.Generic;

namespace Zenviro.Bushido
{
    public class AppModel
    {
        public string Name { get; set; }
        public string Environment { get; set; }
        public ApplicationEnvironmentModel ApplicationEnvironment { get; set; }
        public string Role { get; set; }
        public HostModel Host { get; set; }
        public AssemblyModel MainAssembly { get; set; }
        public List<AssemblyModel> Dependencies { get; set; }
        public WebsiteModel Website { get; set; }
        public WindowsServiceModel WindowsService { get; set; }
        public List<DatabaseConnectionModel> DatabaseConnections { get; set; }
        public List<EndpointConnectionModel> EndpointConnections { get; set; }
        public string Url { get; set; }
    }
}
