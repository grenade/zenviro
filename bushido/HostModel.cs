using System;
using System.Linq;

namespace Zenviro.Bushido
{
    public class HostModel
    {
        public string Name { get; set; }
        public string Domain { get; set; }

        public HostModel() {}

        public HostModel(string text)
            : this(ParseHost(text)) {}

        public HostModel(HostModel host)
            : this(host.Name, host.Domain) {}

        public HostModel(string name, string domain)
        {
            Name = name;
            Domain = domain;
        }

        public override string ToString()
        {
            return string.IsNullOrWhiteSpace(Domain)
                ? Name
                : string.Concat(Name, '.', Domain);
        }

        private static HostModel ParseHost(string text)
        {
            if (text.Contains("://"))
                text = new Uri(text).Host;
            return text.Contains(".")
                ? new HostModel(text.Split('.').First().ToLowerInvariant(), text.Substring(text.IndexOf('.') + 1).ToLowerInvariant())
                : new HostModel(text, null);
        }

        public static bool operator == (HostModel a, HostModel b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a.Name.Equals(b.Name, StringComparison.InvariantCultureIgnoreCase)
                && a.Domain.Equals(b.Domain, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool operator != (HostModel a, HostModel b)
        {
            return !(a == b);
        }
    }
}
