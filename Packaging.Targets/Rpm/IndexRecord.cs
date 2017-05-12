using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Rpm
{
    internal class IndexRecord
    {
        public IndexHeader Header
        {
            get;
            set;
        }

        public object Value
        {
            get;
            set;
        }

        public override string ToString()
        {
            return $"{this.Value} ({this.Header.Type})";
        }
    }
}
