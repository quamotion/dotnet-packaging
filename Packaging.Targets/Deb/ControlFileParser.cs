using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Packaging.Targets.Deb
{
    /// <summary>
    /// Supports reading and writing Debian control files.
    /// </summary>
    /// <seealso href="https://www.debian.org/doc/debian-policy/ch-controlfields.html"/>
    internal static class ControlFileParser
    {
        /// <summary>
        /// Reads a control file from a <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">
        /// A <see cref="Stream"/> which represents the control file.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{TKey, TValue}"/> which represents the contents of the control file.
        /// </returns>
        internal static Dictionary<string, string> Read(Stream stream)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8, false, bufferSize: 1024, leaveOpen: true))
            {
                string line;
                string currentKey = null;

                while (reader.Peek() > 0)
                {
                    line = reader.ReadLine();

                    if (line.StartsWith("#"))
                    {
                        continue;
                    }

                    if (line.StartsWith(" ") || line.StartsWith("\t"))
                    {
                        // Continuation line
                        var value = values[currentKey];
                        value += '\n';

                        if (line.Trim() != ".")
                        {
                            value += line.Trim();
                        }

                        values[currentKey] = value;
                    }
                    else
                    {
                        string[] parts = line.Split(new char[] { ':' }, 2);
                        currentKey = parts[0].Trim();
                        string value = parts[1].Trim();

                        values.Add(currentKey, value);

                    }
                }
            }

            return values;
        }
    }
}
