namespace Zenviro.Bushido
{
    public enum AppChange
    {
        Deployed,
        Removed,
        VersionChanged,
        DependencyChanged,
        WebsiteChanged,
        WebsiteApplicationChanged,
        WebsiteApplicationPoolChanged,
        WebsiteBindingChanged,
        WindowsServiceChanged,
        EndpointChanged,
        DatabaseConnectionChanged
    }
}