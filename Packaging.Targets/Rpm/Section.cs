using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Rpm
{
    internal class Section<K>
    {
        public RpmHeader Header
        {
            get;
            set;
        }

        public Dictionary<K, IndexRecord> Records
        { get; set; } = new Dictionary<K, IndexRecord>();
    }
}
