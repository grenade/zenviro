using System;

namespace Zenviro.Bushido
{
    public class WebsiteApplicationPoolModel
    {
        public string Name { get; set; }
        public string RuntimeVersion { get; set; }
        public string PipelineMode { get; set; }
        public string Username { get; set; }

        #region equality overloads

        public static bool operator ==(WebsiteApplicationPoolModel a, WebsiteApplicationPoolModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(WebsiteApplicationPoolModel a, WebsiteApplicationPoolModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as WebsiteApplicationPoolModel;
            return Equals(o);
        }

        public bool Equals(WebsiteApplicationPoolModel m)
        {
            return m != null
                   && (Name == m.Name || Name.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase))
                   && (RuntimeVersion == m.RuntimeVersion || RuntimeVersion.Equals(m.RuntimeVersion, StringComparison.InvariantCultureIgnoreCase))
                   && (PipelineMode == m.PipelineMode || PipelineMode.Equals(m.PipelineMode, StringComparison.InvariantCultureIgnoreCase))
                   && (Username == m.Username || Username.Equals(m.Username, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode()
                   ^ RuntimeVersion.GetHashCode()
                   ^ PipelineMode.GetHashCode()
                   ^ Username.GetHashCode();
        }

        #endregion
    }
}