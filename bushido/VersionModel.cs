using System;
using NodaTime;

namespace Zenviro.Bushido
{
    public class VersionModel
    {
        public Version AssemblyVersion { get; set; }

        public string FileVersion { get; set; }

        public string ProductVersion { get; set; }

        public ZonedDateTime CompileDate { get; set; }

        #region equality overloads

        public static bool operator ==(VersionModel a, VersionModel b)
        {
            return ((object)a == null && (object)b == null)
                || (((object)a != null) && ((object)b != null) && a.Equals(b))
                || (((object)a == null) || ((object)b == null));
        }

        public static bool operator !=(VersionModel a, VersionModel b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            var o = obj as VersionModel;
            return Equals(o);
        }

        public bool Equals(VersionModel m)
        {
            return m != null
                && FileVersion.Equals(m.FileVersion)
                && ProductVersion.Equals(m.ProductVersion)
                && AssemblyVersion.Equals(m.AssemblyVersion)
                && CompileDate.Equals(m.CompileDate);
        }

        public override int GetHashCode()
        {
            return FileVersion.GetHashCode()
                ^ ProductVersion.GetHashCode()
                ^ AssemblyVersion.GetHashCode()
                ^ CompileDate.GetHashCode();
        }

        #endregion
    }
}