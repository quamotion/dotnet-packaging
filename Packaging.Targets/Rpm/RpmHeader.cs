using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Rpm
{
    internal struct RpmHeader
    {
        /// <summary>
        /// Value identifying this record as an RPM header record. This value shall be "\216\255\350\001".
        /// </summary>
        public uint Magic;

        /// <summary>
        /// Reserved space. This value shall be "\000\000\000\000".
        /// </summary>
        public uint Reserved;

        /// <summary>
        /// The number of Index Records that follow this Header Record. There should be at least 1 Index Record.
        /// </summary>
        public uint IndexCount;

        /// <summary>
        /// The size in bytes of the storage area for the data pointed to by the Index Records.
        /// </summary>
        public uint HeaderSize;
    }
}
