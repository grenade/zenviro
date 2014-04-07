using System;

namespace Zenviro.Bushido
{
    public class ApplicationEnvironmentModel
    {
        public ApplicationGroupModel ApplicationGroup { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        #region equality overloads

        public static bool operator ==(ApplicationEnvironmentModel a, ApplicationEnvironmentModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(ApplicationEnvironmentModel a, ApplicationEnvironmentModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as ApplicationEnvironmentModel;
            return Equals(o);
        }

        public bool Equals(ApplicationEnvironmentModel m)
        {
            return m != null
                && ApplicationGroup.Equals(m.ApplicationGroup)
                && (Name == m.Name || Name.Equals(m.Name, StringComparison.InvariantCultureIgnoreCase))
                && (Code == m.Code || Code.Equals(m.Code, StringComparison.InvariantCultureIgnoreCase))
                && (Description == m.Description || Description.Equals(m.Description, StringComparison.InvariantCultureIgnoreCase));
        }

        public override int GetHashCode()
        {
            return ApplicationGroup.GetHashCode()
                ^ Name.GetHashCode()
                ^ Code.GetHashCode()
                ^ Description.GetHashCode();
        }

        #endregion
    }
}