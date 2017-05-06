using Packaging.Targets.Rpm;

namespace Packaging.Targets.Tests.Rpm
{
    /// <summary>
    /// A test harness for the <see cref="RpmMetadata"/> class; makes some methods public.
    /// </summary>
    internal class PublicRpmMetadata : RpmMetadata
    {
        public PublicRpmMetadata(RpmPackage package) : base(package)
        {
        }

        public void SetStringArrayPublic(IndexTag tag, string[] value)
        {
            base.SetStringArray(tag, value);
        }
    }
}
