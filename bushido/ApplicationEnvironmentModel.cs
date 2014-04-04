using System;

namespace Zenviro.Bushido
{
    public class ApplicationEnvironmentModel
    {
        public ApplicationGroupModel ApplicationGroup { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static bool operator ==(ApplicationEnvironmentModel a, ApplicationEnvironmentModel b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a.Code.Equals(b.Code, StringComparison.InvariantCultureIgnoreCase)
                   && a.ApplicationGroup == b.ApplicationGroup;
        }

        public static bool operator !=(ApplicationEnvironmentModel a, ApplicationEnvironmentModel b)
        {
            return !(a == b);
        }
    }
}