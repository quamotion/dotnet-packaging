using System.Collections.Generic;
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

        public Dictionary<IndexTag, IndexRecord> Records
        {
            get;
            set;
        } = new Dictionary<IndexTag, IndexRecord>();

        public byte[] Data
        {
            get;
            set;
        }
    }
}
