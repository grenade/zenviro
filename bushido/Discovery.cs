using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading.Tasks;
using System.Xml.Linq;
using log4net;
using Newtonsoft.Json;

namespace Zenviro.Bushido
{
    public static class Discovery
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Discovery));

        private static readonly ParallelOptions ParallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount - 1, 1)
        };

        public static void BuildApi()
        {
            const int historyLimit = 20;
            Log.Info("Discovery.BuildApi()");
            var apiDir = Path.Combine(AppConfig.DataDir, "api");
            var appDir = Path.Combine(apiDir, "app");
            var pathDir = Path.Combine(apiDir, "path");
            var historyDir = Path.Combine(apiDir, "history");
            if (Directory.Exists(appDir))
                Directory.Delete(appDir, true);
            if (Directory.Exists(pathDir))
                Directory.Delete(pathDir, true);
            Directory.CreateDirectory(apiDir);
            Directory.CreateDirectory(appDir);
            Directory.CreateDirectory(pathDir);
            Directory.CreateDirectory(historyDir);

            var history = Git.Instance.GetSnaphostHistory().OrderBy(x => x.When).Reverse().ToList();
            File.WriteAllText(Path.Combine(apiDir, "history.json"), JsonConvert.SerializeObject(history.GetRange(0, historyLimit)));
            foreach (var env in history.Select(x => x.App.Environment).Distinct())
            {
                var envHistory = history.Where(x => x.App.Environment == env).ToList();
                if (envHistory.Count > historyLimit)
                    envHistory = envHistory.GetRange(0, historyLimit);
                File.WriteAllText(Path.Combine(historyDir, string.Concat(env, ".json")), JsonConvert.SerializeObject(envHistory));
            }
                

            var apps = DataAccess.GetApps().Select(x => new {x.Name, x.Role}).Distinct();
            var paths = DataAccess.GetPaths();
            File.WriteAllText(Path.Combine(apiDir, "app-names.json"), JsonConvert.SerializeObject(apps));
            File.WriteAllText(Path.Combine(apiDir, "host-names.json"), JsonConvert.SerializeObject(DataAccess.GetHosts().Select(x => x.ToString()).Distinct()));
            foreach (var app in apps)
            {
                var name = app.Name;
                File.WriteAllText(Path.Combine(appDir, string.Concat(name, ".json")), JsonConvert.SerializeObject(DataAccess.GetApps(name)));
                File.WriteAllText(Path.Combine(historyDir, string.Concat(name, ".json")), JsonConvert.SerializeObject(history.Where(x => x.App.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))));
            }
            File.WriteAllText(Path.Combine(apiDir, "paths.json"), JsonConvert.SerializeObject(paths));
            File.WriteAllText(Path.Combine(apiDir, "env-codes.json"), JsonConvert.SerializeObject(DataAccess.GetEnvs()));
            foreach (var path in paths)
                File.WriteAllText(Path.Combine(pathDir, string.Concat(path.Share.TrimStart('\\').Replace("$", string.Empty).Replace('\\', '.'), ".json")), JsonConvert.SerializeObject(path));
            File.WriteAllText(Path.Combine(appDir, "web.json"), JsonConvert.SerializeObject(DataAccess.GetApps().Where(x => x.Role == "web").OrderBy(x => x.Name)));
            File.WriteAllText(Path.Combine(appDir, "svc.json"), JsonConvert.SerializeObject(DataAccess.GetApps().Where(x => x.Role == "svc").OrderBy(x => x.Name)));
        }

        public static void DiscoverApps()
        {
            Log.Info("Discovery.DiscoverApps()");
            var searchPaths = DataAccess.GetPaths().Where(x => !string.IsNullOrWhiteSpace(x.Environment)).ToList();
            var hosts = DataAccess.GetHosts().ToArray();
            Parallel.ForEach(hosts, ParallelOptions, h =>
                DiscoverHostApps(searchPaths.Where(x => x.Host == h).ToList()));              
        }

        private static void DiscoverHostApps(List<SearchPathModel> searchPaths)
        {
            if (searchPaths.Any())
            {
                Log.Info(string.Format("Discovery.DiscoverHostApps({0})", searchPaths.First().Host));
                Parallel.ForEach(searchPaths.Where(x => Directory.Exists(x.Share)), ParallelOptions, searchPath =>
                    Parallel.ForEach(Directory.GetDirectories(searchPath.Share), ParallelOptions, appPath =>
                        DiscoverApp(searchPath, appPath)));
            }
        }

        public static void DiscoverApp(SearchPathModel searchPath, string appPath)
        {
            var assemblyPath = searchPath.Role == "svc"
                ? Analysis.GetSvcAssemblyPath(appPath)
                : Analysis.GetWebAssemblyPath(appPath);
            if (!string.IsNullOrWhiteSpace(assemblyPath)
                && File.Exists(assemblyPath)
                && DataAccess.GetPrefixes().Any(x => Path.GetFileName(assemblyPath).StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
            {
                var mainAssembly = AssemblyInformation.GetAssemblyModel(assemblyPath);
                var app = new AppModel
                {
                    Name = mainAssembly.Name,
                    Role = searchPath.Role,
                    Environment = searchPath.Environment,
                    ApplicationEnvironment = new ApplicationEnvironmentModel { Code = searchPath.Environment },
                    Host = searchPath.Host,
                    MainAssembly = mainAssembly,
                    Dependencies = Analysis.GetDependencies(assemblyPath).ToList()
                };
                Analysis.LinkWebsite(searchPath, app);
                Analysis.LinkWindowsService(searchPath, app);
                Analysis.LinkDatabaseConnections(app);
                Analysis.LinkEndpointConnections(app);

                var file = Path.Combine(AppConfig.DataDir, "snapshot", app.Environment, app.Host.ToString(), string.Concat(app.Name, ".json"));
                var dir = Path.GetDirectoryName(file);
                if (dir != null && !Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.WriteAllText(file, JsonConvert.SerializeObject(app, Formatting.Indented));
            }
        }

        public static void DiscoverSites()
        {
            Log.Info("Discovery.DiscoverSites()");
            var siteDataDir = Path.Combine(AppConfig.DataDir, "infrastructure", "site");
            if (!Directory.Exists(siteDataDir))
                Directory.CreateDirectory(siteDataDir);
            Parallel.ForEach(DataAccess.GetHosts(), ParallelOptions, host => DiscoverHostSites(host, siteDataDir));
        }

        private static void DiscoverHostSites(HostModel host, string siteDataDir)
        {
            try
            {
                var appHostConfigFile = string.Format(@"\\{0}\c$\Windows\System32\inetsrv\config\applicationHost.config", host);
                var xml = XDocument.Load(appHostConfigFile);
                var appPools = xml.GetApplicationPools().ToList();
                var sites = xml.GetWebsites(host).ToList();
                foreach (var site in sites)
                {
                    Log.Debug(string.Format("Processing website: {0}, on host: {1}", site.Name, site.Host));
                    site.ApplicationPools = appPools.Where(p => site.Applications.Select(a => a.ApplicationPool).Any(x => p.Name == x)).ToList();
                    var file = Path.Combine(siteDataDir, string.Concat(host.Name, '.', host.Domain, '.', site.Id, ".json"));
                    File.WriteAllText(file, JsonConvert.SerializeObject(site, Formatting.Indented));
                }
            }
            catch (Exception ex)
            {
                Log.Warn(string.Format("Failed to get website info from host: {0}", host));
                Log.Error(ex);
            }
        }

        public static void DiscoverServices()
        {
            Log.Info("Discovery.DiscoverServices()");
            var serviceDataDir = Path.Combine(AppConfig.DataDir, "infrastructure", "service");
            if (!Directory.Exists(serviceDataDir))
                Directory.CreateDirectory(serviceDataDir);
            Parallel.ForEach(DataAccess.GetHosts(), ParallelOptions, host => DiscoverHostServices(host, serviceDataDir));
        }

        private static void DiscoverHostServices(HostModel host, string serviceDataDir)
        {
            var paths = DataAccess.GetPaths().Where(x => x.Host == host).Select(x => x.Path);
            Log.Debug(string.Format("Service probe: {0}.{1}.", host.Name, host.Domain));
            try
            {
                var scope = new ManagementScope(string.Format(@"\\{0}\root\cimv2:Win32_Service", host));
                var mc = new ManagementClass(scope, new ManagementPath("Win32_Service"), new ObjectGetOptions());
                var services = new List<WindowsServiceModel>(
                    mc.GetInstances().Cast<ManagementBaseObject>().Select(smo => new WindowsServiceModel
                    {
                        Host = host,
                        Name = smo.GetPropertyValue("Name").ToString(),
                        DisplayName = smo.GetPropertyValue("DisplayName").ToString(),
                        Path = smo.GetPropertyValue("PathName").ToString(),
                        StartMode = smo.GetPropertyValue("StartMode").ToString(),
                        Username = smo.GetPropertyValue("StartName").ToString(),
                        State = smo.GetPropertyValue("State").ToString()
                    }))
                    .Where(x => paths.Any(p => x.Path.Contains(p, StringComparison.InvariantCultureIgnoreCase)))
                    .OrderBy(x => x.Name);
                foreach (var service in services)
                    Log.Debug(string.Format("Service discovered: Host: {0}, Name: {1}, State: {2}.", host, service.Name, service.State));
                var file = Path.Combine(serviceDataDir, string.Concat(host.Name, '.', host.Domain, ".json"));
                File.WriteAllText(file, JsonConvert.SerializeObject(services, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Log.Warn(string.Format("Failed to retrieve services on host: {0}.", host));
                Log.Error(ex);
            }
        }
    }
}
