using System;
using System.Xml.Linq;

namespace Packaging.Targets.Pkg
{
    internal class FinderCreateTime
    {
        private readonly XElement element;

        /// <summary>
        /// Initializes a new instance of the <see cref="FinderCreateTime"/> class.
        /// </summary>
        /// <param name="element">
        /// The underlying <see cref="XElement"/>.
        /// </param>
        public FinderCreateTime(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            this.element = element;
        }

        /// <summary>
        /// Gets the number of nano seconds since the reference time.
        /// </summary>
        public long NanoSeconds
        {
            get
            {
                return (long)this.element.Element("nanoseconds");
            }
        }

        /// <summary>
        /// Gets the reference time.
        /// </summary>
        public DateTime Time
        {
            get
            {
                return (DateTime)this.element.Element("time");
            }
        }
    }
}
