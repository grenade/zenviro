namespace Zenviro.Bushido
{
    public class WindowsServiceModel
    {
        public HostModel Host { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
        public string Username { get; set; }
        public string State { get; set; }
        public string StartMode { get; set; }
    }
}
