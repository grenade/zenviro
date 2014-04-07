using System;

namespace Zenviro.Bushido
{
    public class DatabaseConnectionModel
    {
        public string ConnectionString { get; set; }
        public HostModel Host { get; set; }
        public string Provider { get; set; }
        public string Instance { get; set; }
        public int? Port { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }

        #region equality overloads

        public static bool operator ==(DatabaseConnectionModel a, DatabaseConnectionModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(DatabaseConnectionModel a, DatabaseConnectionModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as DatabaseConnectionModel;
            return Equals(o);
        }

        public bool Equals(DatabaseConnectionModel m)
        {
            return m != null
                && Host.Equals(m.Host)
                && Port == m.Port
                && (ConnectionString == m.ConnectionString || ConnectionString.Equals(m.ConnectionString, StringComparison.InvariantCultureIgnoreCase))
                && (Provider == m.Provider || Provider.Equals(m.Provider, StringComparison.InvariantCultureIgnoreCase))
                && (Instance == m.Instance || Instance.Equals(m.Instance, StringComparison.InvariantCultureIgnoreCase))
                && (Database == m.Database || Database.Equals(m.Database, StringComparison.InvariantCultureIgnoreCase))
                && (Username == m.Username || Username.Equals(m.Username, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode()
                ^ ConnectionString.GetHashCode()
                ^ Provider.GetHashCode()
                ^ Instance.GetHashCode()
                ^ Database.GetHashCode()
                ^ Username.GetHashCode();
        }

        #endregion
    }
}
