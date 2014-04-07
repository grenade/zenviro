using System;

namespace Zenviro.Bushido
{
    public class WindowsServiceModel
    {
        public HostModel Host { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Path { get; set; }
        public string Username { get; set; }
        public string State { get; set; }
        public string StartMode { get; set; }

        #region equality overloads

        public static bool operator ==(WindowsServiceModel a, WindowsServiceModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(WindowsServiceModel a, WindowsServiceModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as WindowsServiceModel;
            return Equals(o);
        }

        public bool Equals(WindowsServiceModel m)
        {
            return m != null
                && Host.Equals(m.Host)
                && (Name == m.Name || Name.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase))
                && (DisplayName == m.DisplayName || DisplayName.Equals(m.DisplayName, StringComparison.InvariantCultureIgnoreCase))
                && (Path == m.Path || Path.Equals(m.Path, StringComparison.InvariantCultureIgnoreCase))
                && (Username == m.Username || Username.Equals(m.Username, StringComparison.InvariantCultureIgnoreCase))
                && (State == m.State || State.Equals(m.State, StringComparison.InvariantCultureIgnoreCase))
                && (StartMode == m.StartMode || StartMode.Equals(m.StartMode, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode()
                ^ Name.GetHashCode()
                ^ DisplayName.GetHashCode()
                ^ Path.GetHashCode()
                ^ DisplayName.GetHashCode()
                ^ Username.GetHashCode()
                ^ State.GetHashCode()
                ^ StartMode.GetHashCode();
        }

        #endregion
    }
}
