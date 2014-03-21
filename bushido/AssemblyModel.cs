namespace Zenviro.Bushido
{
    public class AssemblyModel
    {
        public string Name { get; set; }
        public string ProductName { get; set; }
        public string CompanyName { get; set; }
        public bool IsDebug { get; set; }
        public bool IsPreRelease { get; set; }
        public string Path { get; set; }
        public VersionModel Version { get; set; }
    }
}
