using System;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Represents an entry in the package changelog.
    /// </summary>
    internal class ChangelogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangelogEntry"/> class.
        /// </summary>
        public ChangelogEntry()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangelogEntry"/> class, and propopulates the fields.
        /// </summary>
        /// <param name="date">
        /// The date at which the entry was created.
        /// </param>
        /// <param name="name">
        /// The name and e-mail address of the author.
        /// </param>
        /// <param name="text">
        /// A description fo the entry.
        /// </param>
        public ChangelogEntry(DateTimeOffset date, string name, string text)
        {
            this.Date = date;
            this.Name = name;
            this.Text = text;
        }

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
