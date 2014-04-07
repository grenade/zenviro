using System;

namespace Zenviro.Bushido
{
    public class WebsiteApplicationModel
    {
        public string Path { get; set; }
        public string PhysicalPath { get; set; }
        public string ApplicationPool { get; set; }

        #region equality overloads

        public static bool operator ==(WebsiteApplicationModel a, WebsiteApplicationModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(WebsiteApplicationModel a, WebsiteApplicationModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as WebsiteApplicationModel;
            return Equals(o);
        }

        public bool Equals(WebsiteApplicationModel m)
        {
            return m != null
                   && (Path == m.Path || Path.Equals(m.Path, StringComparison.InvariantCultureIgnoreCase))
                   && (PhysicalPath == m.PhysicalPath || PhysicalPath.Equals(m.PhysicalPath, StringComparison.InvariantCultureIgnoreCase))
                   && (ApplicationPool == m.ApplicationPool || ApplicationPool.Equals(m.ApplicationPool, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode()
                   ^ PhysicalPath.GetHashCode()
                   ^ ApplicationPool.GetHashCode();
        }

        #endregion
    }
}