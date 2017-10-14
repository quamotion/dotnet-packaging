using System;
using System.Xml.Linq;

namespace Packaging.Targets.Pkg
{
    internal class FinderCreateTime
    {
        private readonly XElement element;

        public FinderCreateTime(XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            this.element = element;
        }

        public long NanoSeconds
        {
            get
            {
                return (long)this.element.Element("nanoseconds");
            }
        }

        public DateTime Time
        {
            get
            {
                return (DateTime)this.element.Element("time");
            }
        }
    }
}
