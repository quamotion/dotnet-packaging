using System.Collections.ObjectModel;

namespace Packaging.Targets.Rpm
{
    internal class Section
    {
        public RpmHeader Header
        {
            get;
            set;
        }

        public Collection<IndexRecord> Records
        {
            get;
            set;
        } = new Collection<IndexRecord>();

        public byte[] Data
        {
            get;
            set;
        }
    }
}
