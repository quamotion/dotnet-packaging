using System.Collections.ObjectModel;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// A list of pointers.
    /// </summary>
    internal class BomPointerList
    {
        /// <summary>
        /// Gets or sets the number of pointers in the list.
        /// </summary>
        public int NumberOfPointers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets all the pointers.
        /// </summary>
        public Collection<BomPointer> Pointers
        { get; } = new Collection<BomPointer>();
    }
}
