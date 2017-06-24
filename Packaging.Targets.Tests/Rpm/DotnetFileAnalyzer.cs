using Packaging.Targets.IO;
using Packaging.Targets.Rpm;

namespace Packaging.Targets.Tests.Rpm
{
    internal class DotnetFileAnalyzer : FileAnalyzer
    {
        public override string DetermineClass(ArchiveEntry entry)
        {
            if (entry.TargetPath.EndsWith(".map"))
            {
                return "ASCII text, with very long lines, with no line terminators";
            }
            else if (entry.TargetPath.EndsWith(".min.css"))
            {
                return "ASCII text, with very long lines, with CRLF line terminators";
            }
            else if (entry.TargetPath.EndsWith("additional-methods.min.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }
            else if (entry.TargetPath.EndsWith("jquery.validate.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }
            else if (entry.TargetPath.EndsWith("jquery.validate.min.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }
            else if(entry.TargetPath.EndsWith("jquery/LICENSE.txt"))
            {
                return "C source, ASCII text, with CRLF line terminators";
            }
            else if (entry.TargetPath.EndsWith(".min.js"))
            {
                return "ASCII text, with very long lines, with CRLF line terminators";
            }
            else if (entry.TargetPath.EndsWith("bootstrap.css"))
            {
                return "C++ source, ASCII text, with very long lines, with CRLF line terminators";
            }
            else if (entry.TargetPath.EndsWith("additional-methods.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }

            var @class = base.DetermineClass(entry);

            if (@class == "ASCII text")
            {
                return "ASCII text, with CRLF line terminators";
            }
            else if (@class == "UTF-8 Unicode text")
            {
                return "UTF-8 Unicode text, with CRLF line terminators";
            }

            return @class;
        }

        public override RpmFileFlags DetermineFlags(ArchiveEntry entry)
        {
            return RpmFileFlags.None;
        }
    }
}
