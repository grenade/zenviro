using System;
using System.ComponentModel;
using NodaTime;

namespace Zenviro.Bushido
{
    public class VersionModel
    {
        [DisplayName("Assembly Version")]
        public Version AssemblyVersion { get; set; }

        [DisplayName("File Version")]
        public string FileVersion { get; set; }

        [DisplayName("Product Version")]
        public string ProductVersion { get; set; }

        [DisplayName("Compile Date")]
        public ZonedDateTime CompileDate { get; set; }
    }
}