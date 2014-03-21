using System.Collections.Generic;

namespace Zenviro.Bushido
{
    public class WebsiteModel
    {
        public HostModel Host { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WebsiteApplicationModel> Applications { get; set; }
        public List<WebsiteBindingModel> Bindings { get; set; }
        public List<WebsiteApplicationPoolModel> ApplicationPools { get; set; }
    }

    public class WebsiteApplicationModel
    {
        public string Path { get; set; }
        public string PhysicalPath { get; set; }
        public string ApplicationPool { get; set; }
    }

    public class WebsiteApplicationPoolModel
    {
        public string Name { get; set; }
        public string RuntimeVersion { get; set; }
        public string PipelineMode { get; set; }
        public string Username { get; set; }
    }

    public class WebsiteBindingModel
    {
        public string Protocol { get; set; }
        public string BindingInformation { get; set; }
    }
}
