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

        #region equality overloads

        public static bool operator ==(HostModel a, HostModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(HostModel a, HostModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as HostModel;
            return Equals(o);
        }

        public bool Equals(HostModel m)
        {
            return m != null
                && Name.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase)
                && Domain.Equals(m.Domain, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode()
                ^ Domain.GetHashCode();
        }

        #endregion
    }
}
