namespace Packaging.Targets.IO
{
    /// <summary>
    /// Specifies how the <see cref="NativeMethods.dlopen(string, DlOpenFlags)"/> function loads
    /// a dynamic link library.
    /// </summary>
    internal enum DlOpenFlags : int
    {
        /// <summary>
        /// Perform lazy binding. Only resolve symbols as the code that references them is executed.
        /// If the symbol is never referenced, then it is never resolved.
        /// (Lazy binding is only performed for function references; references to variables are always
        /// immediately bound when the library is loaded.)
        /// </summary>
        RTLD_LAZY = 0x00001,

        /// <summary>
        /// If this value is specified, or the environment variable <c>LD_BIND_NOW</c> is set to a nonempty string,
        /// all undefined symbols in the library are resolved before <see cref="NativeMethods.dlopen(string, DlOpenFlags)"/> returns.
        /// If this cannot be done, an error is returned.
        /// </summary>
        RTLD_NOW = 0x00002,

        /// <summary>
        /// The symbols defined by this library will be made available for symbol resolution of subsequently
        /// loaded libraries.
        /// </summary>
        RTLD_GLOBAL = 0x00100,

        /// <summary>
        /// This is the converse of <see cref="RTLD_GLOBAL"/>, and the default if neither flag is specified.
        /// Symbols defined in this library are not made available to resolve references in subsequently
        /// loaded libraries.
        /// </summary>
        RTLD_LOCAL = 0,

        /// <summary>
        /// Do not unload the library during <see cref="NativeMethods.dlclose"/>. Consequently, the library's
        /// static variables are not reinitialized if the library is reloaded with
        /// <see cref="NativeMethods.dlopen"/> at a later time.
        /// </summary>
        /// <remarks>
        /// This flag is not specified in POSIX.1-2001.
        /// </remarks>
        RTLD_NODELETE = 4096,

        /// <summary>
        /// Don't load the library. This can be used to test if the library is already resident
        /// (<see cref="NativeMethods.dlopen"/> returns <see cref="IntPtr.Zero"/> if it is not, or the library's
        /// handle if it is resident). This flag can also be used to promote the flags on a library
        /// that is already loaded. For example, a library that was previously loaded with
        /// <see cref="RTLD_LOCAL"/> can be reopened with <see cref="RTLD_NOLOAD"/> |
        /// <see cref="RTLD_GLOBAL"/>.
        /// </summary>
        /// <remarks>
        /// This flag is not specified in POSIX.1-2001.
        /// </remarks>
        RTLD_NOLOAD = 4,

        /// <summary>
        /// Place the lookup scope of the symbols in this library ahead of the global scope.
        /// This means that a self-contained library will use its own symbols in preference to global
        /// symbols with the same name contained in libraries that have already been loaded.
        /// </summary>
        /// <remarks>
        /// This flag is not specified in POSIX.1-2001.
        /// </remarks>
        RTLD_DEEPBIND = 8
    }
}
