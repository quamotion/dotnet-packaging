using Joveler.Compression.XZ;

namespace Packaging.Targets.IO
{
    /// <summary>
    /// Helper class to ensure XZ library is initialized once globally
    /// </summary>
    internal static class XZHelper
    {
        static XZHelper()
        {
            XZInit.GlobalInit();
        }

        /// <summary>
        /// Ensures XZ library is initialized. This is a no-op after the first call
        /// since initialization happens in static constructor.
        /// </summary>
        public static void EnsureInitialized()
        {
            // Static constructor will have already run by the time this method is called
        }
    }
}
