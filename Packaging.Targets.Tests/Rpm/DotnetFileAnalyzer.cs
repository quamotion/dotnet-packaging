using Packaging.Targets.Rpm;
using System;
using System.Collections.Generic;
using System.Text;
using Packaging.Targets.IO;

namespace Packaging.Targets.Tests.Rpm
{
    internal class DotnetFileAnalyzer : FileAnalyzer
    {
        public override string DetermineClass(string filename, CpioHeader fileHeader, byte[] header)
        {
            if (filename.EndsWith(".map"))
            {
                return "ASCII text, with very long lines, with no line terminators";
            }
            else if (filename.EndsWith(".min.css"))
            {
                return "ASCII text, with very long lines, with CRLF line terminators";
            }
            else if (filename.EndsWith("additional-methods.min.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }
            else if (filename.EndsWith("jquery.validate.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }
            else if (filename.EndsWith("jquery.validate.min.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }
            else if(filename.EndsWith("jquery/LICENSE.txt"))
            {
                return "C source, ASCII text, with CRLF line terminators";
            }
            else if (filename.EndsWith(".min.js"))
            {
                return "ASCII text, with very long lines, with CRLF line terminators";
            }
            else if (filename.EndsWith("bootstrap.css"))
            {
                return "C++ source, ASCII text, with very long lines, with CRLF line terminators";
            }
            else if (filename.EndsWith("additional-methods.js"))
            {
                return "UTF-8 Unicode text, with very long lines, with CRLF line terminators";
            }

            var @class = base.DetermineClass(filename, fileHeader, header);

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

        public override RpmFileFlags DetermineFlags(string filename, CpioHeader fileHeader, byte[] header)
        {
            return RpmFileFlags.None;
        }
    }
}
