//-----------------------------------------------------------------------
// <copyright file="EndianAttribute.cs" company="Quamotion">
//     Copyright (c) 2014 Quamotion. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Defines the endianness of a struct. The endianness of a struct defines how the integer-based
    /// values are serialized to disk.
    /// </summary>
    /// <seealso href="http://en.wikipedia.org/wiki/Endianness"/>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Field)]
    public sealed class EndianAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndianAttribute"/> class.
        /// </summary>
        /// <param name="isLittleEndian">
        /// <see langword="true"/> if the struct is to serialize using the Little Endian convention;
        /// otherwise, <see langword="true"/>.
        /// </param>
        public EndianAttribute(bool isLittleEndian)
        {
            this.IsLittleEndian = isLittleEndian;
        }

        /// <summary>
        /// Gets a value indicating whether the current struct uses the Little Endian convention
        /// to serialize words to disk.
        /// </summary>
        public bool IsLittleEndian
        {
            get;
            private set;
        }
    }
}
