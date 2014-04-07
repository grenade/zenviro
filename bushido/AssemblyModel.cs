using System;

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

        #region equality overloads

        public static bool operator ==(AssemblyModel a, AssemblyModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(AssemblyModel a, AssemblyModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as AssemblyModel;
            return Equals(o);
        }

        public bool Equals(AssemblyModel m)
        {
            return m != null
                && Version.Equals(m.Version)
                && IsDebug == m.IsDebug
                && IsPreRelease == m.IsPreRelease
                && (Name == m.Name || Name.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase))
                && (ProductName == m.ProductName || ProductName.Equals(m.ProductName, StringComparison.InvariantCultureIgnoreCase))
                && (CompanyName == m.CompanyName || CompanyName.Equals(m.CompanyName, StringComparison.InvariantCultureIgnoreCase))
                && (Path == m.Path || Path.Equals(m.Path, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return Version.GetHashCode()
                ^ IsDebug.GetHashCode()
                ^ IsPreRelease.GetHashCode()
                ^ Name.GetHashCode()
                ^ ProductName.GetHashCode()
                ^ CompanyName.GetHashCode()
                ^ Path.GetHashCode();
        }

        #endregion
    }
}
