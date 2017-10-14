using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Pkg
{
    internal interface IEntryContainer
    {
        IEnumerable<FileEntry> Entries { get; }
    }
}
