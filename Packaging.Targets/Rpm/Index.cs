using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Rpm
{
    internal struct Index
    {
        public uint Tag;
        public uint Type;
        public uint Offset;
        public uint Count;
    }
}
