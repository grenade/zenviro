using System;

namespace Zenviro.Bushido
{
    public class ApplicationGroupModel
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public static bool operator ==(ApplicationGroupModel a, ApplicationGroupModel b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            return a.Code.Equals(b.Code, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool operator !=(ApplicationGroupModel a, ApplicationGroupModel b)
        {
            return !(a == b);
        }
    }
}