﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json;
using Zenviro.Bushido;

namespace Zenviro.Ninja
{
    public class Monitor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Monitor));

        #region singleton

        static readonly object DiscLock = new object();
        static Monitor _instance;
        public static Monitor Instance
        {
            get
            {
                lock (DiscLock)
                    return _instance ?? (_instance = new Monitor());
            }
        }

        #endregion

        #region schedule checking and service init, run, work, stop

        private static bool _interrupt;
        private static bool _working;

        public void Init()
        {
            Log.Info("zenviro ninja initialising...");
        }

        public void Run()
        {
            Log.Info("zenviro ninja running...");
            while (!_interrupt)
            {
                _working = true;
                Work();
                _working = false;
                Sleep(Schedule.PauseBetweenCycles);
            }
            Cleanup();
        }

        public void Stop()
        {
            Log.Info("zenviro ninja stopping...");
            _interrupt = true;
            while (_working)
            {
                Thread.Sleep(500);
            }
        }

        private void Work()
        {
            if (Schedule.ShouldWork)
            {
                Log.Info("zenviro ninja working...");
                if (Git.Instance.Pull()) //config changes from elsewhere...
                {
                    try
                    {
                        var searchPaths = Directory.GetFiles(Path.Combine(AppConfig.DataDir, "config", "path"), "*.json", SearchOption.AllDirectories)
                            .Select(hostFile => JsonConvert.DeserializeObject<SearchPathModel>(File.ReadAllText(hostFile)));
                        var toWatch = searchPaths.Where(x => Directory.Exists(x.Share)).ToList();
                        //foreach (var key in _watchers.Keys.Where(e => toWatch.Select(x => x.Share.ToLower()).All(x => x != e)))
                        //    Ignore(key);
                        foreach (var searchPath in toWatch)
                            Watch(searchPath);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex);
                    }
                }
            }
            else
            {
                Log.Info("zenviro ninja ignoring work due to scheduling...");
            }
        }

        private void Cleanup()
        {
            Log.Info("Bot cleaning up...");
            foreach (var key in _watchers.Keys)
            {
                Ignore(key);
            }
        }

        private void Sleep(int millisecondsTimeout)
        {
            if (Environment.UserInteractive)
                millisecondsTimeout = 5000;
            Log.Info(string.Format("zenviro ninja sleeping for {0} {1}...", millisecondsTimeout < 60000 ? millisecondsTimeout / 1000 : millisecondsTimeout / 1000 / 60, millisecondsTimeout < 60000 ? "seconds" : "minutes"));
            // http://stackoverflow.com/a/11785656/68115
            var rng = new Random();
            Task.Factory.StartNew(() =>
            {
                while (!_interrupt && !_working)
                {
                    if (Environment.UserInteractive && Console.KeyAvailable)
                    {
                        _interrupt = true;
                        Log.Info("Sleep interrupted by user input.");
                    }
                    var x = rng.Next(2, 30);
                    var zees = new StringBuilder();
                    for (var i = 0; i < x; i++)
                        zees.Append('z');
                    Log.Debug(string.Format("{0}...", zees));
                    Thread.Sleep(2000);
                }
                if (_interrupt)
                    Log.Info("Sleep interrupted by service 'Stop' command.");
            }).Wait(millisecondsTimeout);
        }

        #endregion

        #region filesystem watcher

        readonly Dictionary<string, FileSystemWatcher> _watchers = new Dictionary<string, FileSystemWatcher>();
        Dictionary<string, SearchPathModel> _searchPaths = DataAccess.GetPaths().ToDictionary(x => x.Share.ToLowerInvariant(), x => x);

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Watch(SearchPathModel path)
        {
            if (!_watchers.ContainsKey(path.Share.ToLower()))
            {
                var w = new FileSystemWatcher
                {
                    Path = path.Share,
                    NotifyFilter = NotifyFilters.LastWrite
                                   | NotifyFilters.LastAccess
                                   | NotifyFilters.FileName
                                   | NotifyFilters.DirectoryName,
                    Filter = "*.config"
                };
                w.Changed += OnChanged;
                w.Created += OnChanged;
                w.Deleted += OnChanged;
                w.EnableRaisingEvents = true;
                w.IncludeSubdirectories = true;
                _watchers.Add(path.Share.ToLower(), w);
            }
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void Ignore(string key)
        {
            if (_watchers.ContainsKey(key))
            {
                _watchers[key].Changed -= OnChanged;
                _watchers[key].Created -= OnChanged;
                _watchers[key].Deleted -= OnChanged;
                _watchers[key].IncludeSubdirectories = false;
                _watchers[key].EnableRaisingEvents = false;
                _watchers.Remove(key);
            }
        }

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            var appPath = Path.GetDirectoryName(e.FullPath);
            if (appPath != null)
            {
                var searchPath = _searchPaths[_watchers.Keys.First(x => appPath.ToLower().StartsWith(x))];
                if(searchPath.Role == "svc")
                    Discovery.DiscoverHostServices(searchPath.Host);
                else
                    Discovery.DiscoverHostSites(searchPath.Host);
                var appName = Discovery.DiscoverApp(searchPath, appPath);
                if (!string.IsNullOrWhiteSpace(appName))
                {
                    Git.Instance.AddChanges();
                    //todo: work out which parts of app-specific BuildApi() need to run
                    //Discovery.BuildApi();
                    Discovery.BuildAppHistory(searchPath, appName);
                    Git.Instance.AddChanges();
                }
            }
        }

        #endregion
    }
}
