using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Provides extension methods for the <see cref="Collection{T}"/> class.
    /// </summary>
    internal static class CollectionExtensions
    {
        /// <summary>
        /// Adds multiple values at once.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the values.
        /// </typeparam>
        /// <param name="collection">
        /// The collection to which to add the values.
        /// </param>
        /// <param name="values">
        /// The values to add.
        /// </param>
        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> values)
        {
            foreach(var v in values)
            {
                collection.Add(v);
            }
        }
    }
}
