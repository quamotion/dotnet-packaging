using System.Collections.ObjectModel;

namespace Packaging.Targets.Pkg
{
    internal class BomFileTree : Collection<BomFile>
    {
        public BomTree BomTree
        { get; set; }
    }
}
