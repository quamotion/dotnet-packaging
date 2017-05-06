using System.IO;

namespace Packaging.Targets.Rpm
{
    internal class RpmPackage
    {
        public RpmPackage()
        {
            this.Header = new Section<IndexTag>();
            this.Signature = new Section<SignatureTag>();
        }

        public RpmLead Lead
        {
            get;
            set;
        }

        public Section<IndexTag> Header
        {
            get;
            set;
        }

        public Section<SignatureTag> Signature
        {
            get;
            set;
        }

        public long HeaderOffset
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
