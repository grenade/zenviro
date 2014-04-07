using System.Collections.Generic;
using NodaTime;

namespace Zenviro.Bushido
{
    public class AppPathGitHistoryModel
    {
        public AppModel App { get; set; }
        public string Path { get; set; }
        public string Sha { get; set; }
        public ZonedDateTime When { get; set; }
        public List<AppChange> Changes { get; set; }
    }
}