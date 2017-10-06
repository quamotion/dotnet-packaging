using Packaging.Targets.IO;

namespace Packaging.Targets.Deb
{
    /// <summary>
    /// Needed for entries we don't particularry care about
    /// during package generation, such as `shlibs`
    /// </summary>
    public class DebPackageControlFileData
    {
        public LinuxFileMode Mode { get; set; }

        public string Contents { get; set; }
    }
}