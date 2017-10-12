using Org.BouncyCastle.Bcpg.OpenPgp;
using System.IO;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Common interface for any class that can generate PGP signatures of a <see cref="Stream"/>.
    /// </summary>
    internal interface IPackageSigner
    {
        PgpSignature Sign(Stream payload);
    }
}
