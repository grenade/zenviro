using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace Zenviro.Bushido
{
    public static class DataAccess
    {
        public static IEnumerable<ApplicationEnvironmentModel> GetEnvs()
        {
            return GetApps()
                .Select(x => x.ApplicationEnvironment)
                .Where(x => x != null && !string.IsNullOrWhiteSpace(x.Code))
                .GroupBy(x => string.Concat(x.ApplicationGroup != null && !string.IsNullOrWhiteSpace(x.ApplicationGroup.Code) ? x.ApplicationGroup.Code : string.Empty, x.Code))
                .Select(x => x.First())
                .OrderBy(x => x.Code);
        }

        public static ApplicationEnvironmentModel GetEnv(string code)
        {
            return GetEnvs().FirstOrDefault(x => x.Code == code);
        }

        public static IEnumerable<AppModel> GetApps()
        {
            return Directory.GetFiles(Path.Combine(AppConfig.DataDir, "snapshot"), "*.json", SearchOption.AllDirectories)
                .Select(x => JsonConvert.DeserializeObject<AppModel>(File.ReadAllText(x))).OrderBy(x => x.Name);
        }

        public static IEnumerable<AppModel> GetApps(string search)
        {
            return GetApps().Where(x =>
                x.Host.ToString().Contains(search, StringComparison.InvariantCultureIgnoreCase)
                || x.Name.Contains(search, StringComparison.InvariantCultureIgnoreCase)).OrderBy(x => x.Name);
        }

        public static AppModel GetApp(string host, string name)
        {
            var app = GetApps().SingleOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase) && x.Host.ToString().Equals(host, StringComparison.InvariantCultureIgnoreCase));
            return app;
        }

        public static IEnumerable<SearchPathModel> GetPaths(HostModel host = null)
        {
            var filter = "*.json";
            if (host != null)
                filter = string.Format("{0}.*.json", host);
            return Directory.GetFiles(Path.Combine(AppConfig.DataDir, "config", "path"), filter, SearchOption.AllDirectories)
                .Where(x =>
                {
                    var fileName = Path.GetFileName(x);
                    return fileName != null && !fileName.StartsWith("dummy");
                })
                .Select(x => JsonConvert.DeserializeObject<SearchPathModel>(File.ReadAllText(x)))
                .GroupBy(x => x.Share)//todo: log or cleanup duplicate shares
                .Select(x => x.First())
                .OrderBy(x => string.Concat(x.Host.ToString(), x.Path));
        }

        public static void SavePath(SearchPathModel path)
        {
            var file = Path.Combine(AppConfig.DataDir, "config", "path", string.Concat(path.Host, '.', path.Path.Replace(":", string.Empty).Replace('\\', '.'), ".json"));
            File.WriteAllText(file, JsonConvert.SerializeObject(path, Formatting.Indented));
            Git.Instance.AddChanges();
        }

        public static void DeletePath(SearchPathModel path)
        {
            var file = Path.Combine(AppConfig.DataDir, "config", "path", string.Concat(path.Host, '.', path.Path.Replace(":", string.Empty).Replace('\\', '.'), ".json"));
            if (File.Exists(file))
                File.Delete(file);
        }

        public static IEnumerable<HostModel> GetHosts()
        {
            return GetPaths()
                .Select(x => x.Host)
                .GroupBy(x => x.ToString())
                .Select(x => x.First())
                .OrderBy(x => x.ToString());
        }

        public static IEnumerable<WebsiteModel> GetSites(HostModel host = null)
        {
            var filter = "*.json";
            if (host != null)
                filter = string.Format("{0}.*.json", host);
            return Directory.GetFiles(Path.Combine(AppConfig.DataDir, "infrastructure", "site"), filter, SearchOption.AllDirectories)
                .Select(hostFile => JsonConvert.DeserializeObject<WebsiteModel>(File.ReadAllText(hostFile)))
                .OrderBy(x => string.Concat(x.Host.ToString(), x.Name));
        }

        public static IEnumerable<WindowsServiceModel> GetServices(HostModel host = null)
        {
            var filter = "*.json";
            if (host != null)
                filter = string.Format("{0}.json", host);
            return Directory.GetFiles(Path.Combine(AppConfig.DataDir, "infrastructure", "service"), filter, SearchOption.AllDirectories)
                .SelectMany(hostFile => JsonConvert.DeserializeObject<IEnumerable<WindowsServiceModel>>(File.ReadAllText(hostFile)))
                .OrderBy(x => string.Concat(x.Host.ToString(), x.Name));
        }

        public static IEnumerable<string> GetPrefixes()
        {
            return JsonConvert.DeserializeObject<IEnumerable<string>>(File.ReadAllText(Path.Combine(AppConfig.DataDir, "config", "default", "assembly.startswith.json")));
        } 
    }
}
