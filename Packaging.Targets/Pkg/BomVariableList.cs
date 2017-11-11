using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Pkg
{
    /// <summary>
    /// A list of variables in a BOM file.
    /// </summary>
    internal class BomVariableList
    {
        /// <summary>
        /// Gets or sets the number of variables.
        /// </summary>
        public uint NumberOfVariables
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a list of all variables.
        /// </summary>
        public Dictionary<string, BomVariable> Variables
        { get; } = new Dictionary<string, BomVariable>();
    }
}
