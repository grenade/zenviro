using System;

namespace Zenviro.Bushido
{
    public class WebsiteBindingModel
    {
        public string Protocol { get; set; }
        public string BindingInformation { get; set; }

        #region equality overloads

        public static bool operator ==(WebsiteBindingModel a, WebsiteBindingModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(WebsiteBindingModel a, WebsiteBindingModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as WebsiteBindingModel;
            return Equals(o);
        }

        public bool Equals(WebsiteBindingModel m)
        {
            return m != null
                   && (Protocol == m.Protocol || Protocol.Equals(m.Protocol, StringComparison.InvariantCultureIgnoreCase))
                   && (BindingInformation == m.BindingInformation || BindingInformation.Equals(m.BindingInformation, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return Protocol.GetHashCode()
                   ^ BindingInformation.GetHashCode();
        }

        #endregion
    }
}