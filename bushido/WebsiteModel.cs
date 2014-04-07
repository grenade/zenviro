using System;
using System.Collections.Generic;

namespace Zenviro.Bushido
{
    public class WebsiteModel
    {
        public HostModel Host { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WebsiteApplicationModel> Applications { get; set; }
        public List<WebsiteBindingModel> Bindings { get; set; }
        public List<WebsiteApplicationPoolModel> ApplicationPools { get; set; }

        #region equality overloads

        public static bool operator ==(WebsiteModel a, WebsiteModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(WebsiteModel a, WebsiteModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as WebsiteModel;
            return Equals(o);
        }

        public bool Equals(WebsiteModel m)
        {
            return m != null
                && Host.Equals(m.Host)
                && Id == m.Id
                && (Name == m.Name || Name.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase))
                && Extensions.ListEquals(Applications, m.Applications)
                && Extensions.ListEquals(Bindings, m.Bindings)
                && Extensions.ListEquals(ApplicationPools, m.ApplicationPools);
        }

        public override int GetHashCode()
        {
            return Host.GetHashCode()
                ^ Id.GetHashCode()
                ^ Name.GetHashCode()
                ^ Applications.GetHashCode()
                ^ Bindings.GetHashCode()
                ^ ApplicationPools.GetHashCode();
        }

        #endregion
    }
}
