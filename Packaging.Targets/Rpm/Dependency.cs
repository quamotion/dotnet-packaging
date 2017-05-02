namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Represents a dependency.
    /// </summary>
    internal class Dependency
    {
        /// <summary>
        /// Gets or sets the dependency type.
        /// </summary>
        public IndexTag Type
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the index of the dependency.
        /// </summary>
        public int Index
        {
            get;
            set;
        }
    }
}
