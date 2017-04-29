using System;
using System.Collections.Generic;
using System.Text;

namespace Packaging.Targets.Rpm
{
    internal enum SignatureTag
    {
        /// <summary>
        /// This tag specifies the combined size of the Header and Payload sections.
        /// </summary>
        RPMSIGTAG_SIZE = 1000,

        /// <summary>
        /// This tag specifies the uncompressed size of the Payload archive, including the cpio headers.
        /// </summary>
        RPMSIGTAG_PAYLOADSIZE = 1007,

        /// <summary>
        /// This index contains the SHA1 checksum of the entire Header Section, including the Header Record, Index Records and Header store.
        /// </summary>
        RPMSIGTAG_SHA1 = 269,

        /// <summary>
        /// This tag specifies the 128-bit MD5 checksum of the combined Header and Archive sections.
        /// </summary>
        RPMSIGTAG_MD5 = 1004,

        /// <summary>
        /// The tag contains the DSA signature of the Header section. The data is formatted as a Version 3 Signature Packet as specified in
        /// RFC 2440: OpenPGP Message Format. If this tag is present, then the SIGTAG_GPG tag shall also be present.
        /// </summary>
        RPMSIGTAG_DSA = 267,

        /// <summary>
        /// The tag contains the RSA signature of the Header section.The data is formatted as a Version 3 Signature Packet as specified in
        /// RFC 2440: OpenPGP Message Format. If this tag is present, then the SIGTAG_PGP shall also be present.
        /// </summary>
        RPMSIGTAG_RSA = 268,

        /// <summary>
        /// This tag specifies the RSA signature of the combined Header and Payload sections. The data is formatted as a Version 3 Signature Packet
        /// as specified in RFC 2440: OpenPGP Message Format.
        /// </summary>
        RPMSIGTAG_PGP = 1002,

        /// <summary>
        /// The tag contains the DSA signature of the combined Header and Payload sections. The data is formatted as a Version 3 Signature Packet
        /// as specified in RFC 2440: OpenPGP Message Format.
        /// </summary>
        RPMSIGTAG_GPG = 1005,
    }
}
