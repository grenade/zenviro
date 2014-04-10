using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using log4net;
using Newtonsoft.Json;
using NodaTime;

namespace Zenviro.Bushido
{
    public class Git
    {
        private const string Local = "refs/heads/master";
        private const string Remote = "refs/remotes/origin/master";
        private static readonly ILog Log = LogManager.GetLogger(typeof(Git));

        private static Signature Committer
        {
            get { return new Signature(AppConfig.GitConfigName, AppConfig.GitConfigEmail, DateTimeOffset.Now); }
        }

        static readonly object GitLock = new object();
        static Git _instance;
        public static Git Instance
        {
            get
            {
                lock (GitLock)
                    return _instance ?? (_instance = new Git());
            }
        }

        static readonly object RepoLock = new object();

        public List<AppPathGitHistoryModel> GetSnaphostHistory(string pathFilter = null)
        {
            IEnumerable<AppPathGitHistoryModel> history;
            try
            {
                lock (RepoLock)
                {
                    using (var r = new Repository(AppConfig.DataDir))
                    {
                        var repositoryInfoPath = r.Info.Path;
                        var historyDir = Path.Combine(AppConfig.DataDir, "api", "history");
                        var someHistoryExists = Directory.Exists(historyDir) && Directory.GetFiles(historyDir, "*.json").Any();
                        history = r.Commits
                            //recent
                            .Where(c => (someHistoryExists || (c.Committer.When > DateTimeOffset.Now.AddDays(-14))))
                            //paths with blob files (trees with blobs under snapshot)
                            .SelectMany(c => c.Tree.GetBlobs().Where(x => x.Path.StartsWith("snapshot")
                                //paths containing hostnameFilter
                                && (string.IsNullOrWhiteSpace(pathFilter) || x.Path.Contains(pathFilter, StringComparison.InvariantCultureIgnoreCase))).Select(x => x.Path)).Distinct()
                            //relevant commits: http://stackoverflow.com/a/21707186/68115
                            .SelectMany(p => r.Commits.Where(c => c.Parents.Count() == 1 && c.Tree[p] != null && (c.Parents.FirstOrDefault().Tree[p] == null || c.Tree[p].Target.Id != c.Parents.FirstOrDefault().Tree[p].Target.Id))
                            //history
                            .Select(c => c.ToAppPathGitHistoryModel(p, repositoryInfoPath))).ToList();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
            foreach (var group in history.GroupBy(x=>x.App.MainAssembly.Path))
            {
                var orderedItems = group.OrderBy(x => x.When).ToArray();
                for (var i = 0; i < orderedItems.Length; i++)
                {
                    orderedItems[i].Changes = new List<AppChange>();
                    if (i != 0 && orderedItems[i].App == null)
                        orderedItems[i].Changes.Add(AppChange.Removed);
                    else if (i == 0) //todo: handle situations when we arent looking at full history
                        orderedItems[i].Changes.Add(AppChange.Deployed);
                    else
                    {
                        if (!orderedItems[i].App.MainAssembly.Version.Equals(orderedItems[i - 1].App.MainAssembly.Version))
                            orderedItems[i].Changes.Add(AppChange.Version);
                        if (orderedItems[i].App.Website != orderedItems[i - 1].App.Website)
                        {
                            if (orderedItems[i].App.Website.Id != orderedItems[i - 1].App.Website.Id
                                || (orderedItems[i].App.Website.Name != orderedItems[i - 1].App.Website.Name || !orderedItems[i].App.Website.Name.Equals(orderedItems[i - 1].App.Website.Name, StringComparison.InvariantCultureIgnoreCase))
                                || !orderedItems[i].App.Website.Host.Equals(orderedItems[i - 1].App.Website.Host))
                                orderedItems[i].Changes.Add(AppChange.Website);
                            if (!Extensions.ListEquals(orderedItems[i].App.Website.Applications, orderedItems[i - 1].App.Website.Applications))
                                orderedItems[i].Changes.Add(AppChange.WebApplication);
                            if (!Extensions.ListEquals(orderedItems[i].App.Website.ApplicationPools, orderedItems[i - 1].App.Website.ApplicationPools))
                                orderedItems[i].Changes.Add(!Extensions.ListEquals(orderedItems[i].App.Website.ApplicationPools.Select(x => x.Username ?? "ApplicationPoolIdentity").ToList(), orderedItems[i - 1].App.Website.ApplicationPools.Select(x => x.Username ?? "ApplicationPoolIdentity").ToList())
                                    ? AppChange.ApplicationPoolIdentity
                                    : AppChange.ApplicationPool);
                            if (!Extensions.ListEquals(orderedItems[i].App.Website.Bindings, orderedItems[i - 1].App.Website.Bindings))
                                orderedItems[i].Changes.Add(AppChange.WebsiteBinding);
                        }
                        if (orderedItems[i].App.WindowsService != orderedItems[i - 1].App.WindowsService)
                            orderedItems[i].Changes.Add(!orderedItems[i].App.WindowsService.Username.Equals(orderedItems[i - 1].App.WindowsService.Username, StringComparison.InvariantCultureIgnoreCase) 
                                ? AppChange.WindowsServiceIdentity
                                : AppChange.WindowsService);
                        if (!Extensions.ListEquals(orderedItems[i].App.EndpointConnections, orderedItems[i - 1].App.EndpointConnections))
                            orderedItems[i].Changes.Add(AppChange.EndpointConnections);
                        if (!Extensions.ListEquals(orderedItems[i].App.DatabaseConnections, orderedItems[i - 1].App.DatabaseConnections))
                            orderedItems[i].Changes.Add(AppChange.DatabaseConnections);
                        if (!Extensions.ListEquals(orderedItems[i].App.Dependencies, orderedItems[i - 1].App.Dependencies))
                            orderedItems[i].Changes.Add(AppChange.Dependency);
                    }
                }
                Log.Debug(string.Format("{0}@{1}: {2} history item(s) indexed.", group.First().App.Name, group.First().App.Host, group.Count()));
            }
            return history.OrderBy(x => x.When).Reverse().ToList();
        }

        public void AddChanges()
        {
            try
            {
                lock (RepoLock)
                {
                    using (var r = new Repository(AppConfig.DataDir))
                    {
                        if (r.HasUnstagedChanges())
                        {
                            r.CommitUnstagedChanges(Committer);
                            if (!string.IsNullOrWhiteSpace(AppConfig.GitRemote) && r.Network.Remotes.Any())
                            {
                                r.SyncRemoteBranch();
                                r.Network.Push(r.Head);
                                Log.Info("Configuration pushed to remote git repository.");
                            }
                        }
                    }
                }
            }
            catch (NonFastForwardException e)
            {
                Log.Warn("The remote repository is out of sync with the local repository. Changes have not been synced to remote.");
                Log.Error(e);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public void Clone()
        {
            try
            {
                lock (RepoLock)
                    Repository.Clone(AppConfig.GitRemote, AppConfig.DataDir);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        public bool Pull()
        {
            bool changeDetected = false;
            try
            {
                lock (RepoLock)
                    using (var r = new Repository(AppConfig.DataDir))
                    {
                        r.Fetch("origin", new FetchOptions());
                        changeDetected = r.Branches[Local].Commits.All(x => x.Sha != r.Branches[Remote].Tip.Sha);
                        if (changeDetected)
                        {
                            var result = r.Merge(r.Branches[Remote].Tip, Committer);
                            Log.Info(string.Format("DataDir updated to: {0}, with merge status: {1}.", r.Branches[Local].Tip.Sha.Substring(0, 7), result.Status));
                        }
                        else
                        {
                            Log.Info(string.Format("DataDir is up to date."));
                        }
                    }
            }
            catch (Exception e)
            {
                Log.Error(e);
                //throw;
            }
            return changeDetected;
        }
    }

    public static class GitExtensions
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GitExtensions));
        public static void SyncRemoteBranch(this Repository repository)
        {
            var remote = repository.Network.Remotes.Any(x => x.Name == "origin")
                ? repository.Network.Remotes["origin"]
                : repository.Network.Remotes.Add("origin", AppConfig.GitRemote);
            var canonicalName = repository.Head.CanonicalName;
            repository.Branches.Update(repository.Head,
                b => b.Remote = remote.Name,
                b => b.UpstreamBranch = canonicalName);
        }

        public static bool HasUnstagedChanges(this Repository repository)
        {
            var status = repository.Index.RetrieveStatus();
            return status.Modified.Union(status.Untracked).Union(status.Missing).Any();
        }

        public static void CommitUnstagedChanges(this Repository repository, Signature committer)
        {
            var status = repository.Index.RetrieveStatus();
            var changes = new Dictionary<string, IEnumerable<StatusEntry>>
            {
                { "Untracked", status.Untracked },
                { "Modified", status.Modified },
                { "Missing", status.Missing }
            };
            foreach (var key in changes.Keys.Where(x => changes[x].Any()))
            {
                var paths = changes[key].Select(x => x.FilePath).ToArray();
                Log.Info(string.Format("{0} configuration changes discovered.", paths.Count()));

                foreach (var path in paths)
                {
                    string message;
                    repository.Index.Stage(path);
                    switch (path.Split(Path.DirectorySeparatorChar).First())
                    {
                        case "snapshot":
                            message = string.Format("{0}, {1} {2} env ({3}).",
                                Path.GetFileNameWithoutExtension(path),
                                key == "Missing" ? "removed from" : "deployed to",
                                Path.GetFileName(Path.GetDirectoryName(Path.GetDirectoryName(path))),
                                Path.GetFileName(Path.GetDirectoryName(path)));
                            break;
                        case "config":
                            switch (path.Split(Path.DirectorySeparatorChar)[1])
                            {
                                case "path":
                                    switch (key)
                                    {
                                        case "Untracked":
                                            message = "Search path added.";
                                            break;
                                        case "Modified":
                                            message = "Search path modified.";
                                            break;
                                        default:
                                            message = "Search path removed.";
                                            break;
                                    }
                                    break;
                                default:
                                    message = "Configuration change detected.";
                                    break;
                            }
                            break;
                        default:
                            message = "Configuration change detected.";
                            break;
                    }
                    repository.Commit(message, committer);
                }
                Log.Info("Configuration changes committed to local git repository.");
            }
        }

        public static IEnumerable<TreeEntry> GetBlobs(this Tree tree)
        {
            var blobs = tree.Where(x => x.TargetType == TreeEntryTargetType.Blob).ToList();
            blobs.AddRange(tree.Where(x => x.TargetType == TreeEntryTargetType.Tree).SelectMany(x => (x.Target as Tree).GetBlobs()));
            return blobs;
        }

        public static AppPathGitHistoryModel ToAppPathGitHistoryModel(this Commit commit, string path, string repositoryInfoPath)
        {
            AppModel app;
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", string.Empty).Substring(0, 12));
            var workDir = Path.Combine(tempFolder, "work");
            Directory.CreateDirectory(workDir);
            using (var repo = new Repository(repositoryInfoPath, new RepositoryOptions { WorkingDirectoryPath = workDir, IndexPath = Path.Combine(Path.GetTempPath(), "index") }))
            {
                repo.CheckoutPaths(commit.Sha, new[] { path }, new CheckoutOptions { CheckoutModifiers = CheckoutModifiers.Force });
                var revision = Path.Combine(repo.Info.WorkingDirectory, path);
                app = JsonConvert.DeserializeObject<AppModel>(File.ReadAllText(revision));
            }
            Directory.Delete(tempFolder, true);
            Log.Debug(string.Format("{0} {1} ({2}) {3} {4}.", app.Name, app.Environment, app.Host, commit.Committer.When, commit.Sha));
            return new AppPathGitHistoryModel
            {
                App = app,
                Path = path,
                Sha = commit.Sha,
                When = ZonedDateTime.FromDateTimeOffset(commit.Committer.When)
            };
        }
    }
}
