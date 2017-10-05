using System;
using System.Collections.Generic;

namespace Packaging.Targets.Deb
{
    /// <summary>
    /// Represents a Debian installer package.
    /// </summary>
    internal class DebPackage
    {
        /// <summary>
        /// Gets or sets the Debian installer file format used.
        /// </summary>
        public Version PackageFormatVersion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value of the control file.
        /// </summary>
        public Dictionary<string, string> ControlFile
        {
            get;
            set;
        }
        
        public Dictionary<string, DebPackageControlFileData> ControlExtras { get; set; }
        public Dictionary<string, string> Md5Sums { get; set; }
        
        public string PreInstallScript { get; set; }
        public string PostInstallScript { get; set; }
        public string PreRemoveScript { get; set; }
        public string PostRemoveScript { get; set; }
    }
}
