using System;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Represents an entry in the package changelog.
    /// </summary>
    internal class ChangelogEntry
    {
        /// <summary>
        /// Gets or sets the date at which the entry was created.
        /// </summary>
        public DateTimeOffset Date
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name and e-mail address of the author.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a description of the entry.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.Date} {this.Name}: {this.Text}";
        }
    }
}
