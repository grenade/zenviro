namespace Zenviro.Bushido
{
    public class DatabaseConnectionModel
    {
        public string ConnectionString { get; set; }
        public HostModel Host { get; set; }
        public string Provider { get; set; }
        public string Instance { get; set; }
        public int? Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
    }
}
