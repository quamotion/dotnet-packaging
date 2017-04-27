using System.IO;

namespace Packaging.Targets.Rpm
{
    internal class RpmPackage
    {
        public RpmLead Lead
        {
            get;
            set;
        }

        public Section Header
        {
            get;
            set;
        }

        public Section Signature
        {
            get;
            set;
        }

        public long PayloadOffset
        {
            get;
            set;
        }

        public Stream Stream
        {
            get;
            set;
        }
    }
}
