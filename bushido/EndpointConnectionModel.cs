using System;

namespace Zenviro.Bushido
{
    public class EndpointConnectionModel
    {
        public string Address { get; set; }
        public HostModel Host { get; set; }
        public string Username { get; set; }

        #region equality overloads

        public static bool operator ==(EndpointConnectionModel a, EndpointConnectionModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(EndpointConnectionModel a, EndpointConnectionModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as EndpointConnectionModel;
            return Equals(o);
        }

        public bool Equals(EndpointConnectionModel m)
        {
            return m != null
                && Host.Equals(m.Host)
                && (Address == m.Address || Address.Equals(m.Address, StringComparison.InvariantCultureIgnoreCase))
                && (Username == m.Username || Username.Equals(m.Username, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode()
                ^ Address.GetHashCode()
                ^ Username.GetHashCode();
        }

        #endregion
    }
}
