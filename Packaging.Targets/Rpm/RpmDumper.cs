using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Packaging.Targets.Rpm
{
    /// <summary>
    /// Dumps the contents of the headers of a RPM package to a text file. Useful when comparing two
    /// RPM packages.
    /// </summary>
    internal static class RpmDumper
    {
        public static void Dump(RpmPackage package, string path)
        {
            using (var file = File.Open(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            using (StreamWriter writer = new StreamWriter(file))
            {
                Dump(package, writer);
            }
        }

        public static void Dump(RpmPackage package, StreamWriter writer)
        {
            writer.WriteLine("Lead:");
            writer.WriteLine("  ArchNum        {0}", package.Lead.ArchNum);
            writer.WriteLine("  Magic          {0}", package.Lead.Magic);
            writer.WriteLine("  Major          {0}", package.Lead.Major);
            writer.WriteLine("  Minor          {0}", package.Lead.Minor);
            writer.WriteLine("  Name           {0}", package.Lead.Name);
            writer.WriteLine("  OsNum          {0}", package.Lead.OsNum);
            writer.WriteLine("  SignatureType  {0}", package.Lead.SignatureType);
            writer.WriteLine("  Type           {0}", package.Lead.Type);

            writer.WriteLine();

            writer.WriteLine("Signature:");
            Dump(package.Signature, writer);

            writer.WriteLine("Header:");
            Dump(package.Header, writer);
        }

        public static void Dump<T>(Section<T> section, StreamWriter writer)
        {
            foreach (var record in section.Records)
            {
                writer.WriteLine("  {0}:", record.Key);
                writer.WriteLine("    Count:  {0}", record.Value.Header.Count);
                writer.WriteLine("    Offset: {0}", record.Value.Header.Offset);
                writer.WriteLine("    Tag:    {0}", record.Value.Header.Tag);
                writer.WriteLine("    Type:   {0}", record.Value.Header.Type);

                if (record.Value.Value is IEnumerable<byte>)
                {
                    writer.WriteLine("    Value:   {0}", BitConverter.ToString((byte[])record.Value.Value).Replace("-", string.Empty));
                }
                else if (record.Value.Value is IEnumerable)
                {
                    writer.Write("    Value:   ");
                    bool isFirst = true;

                    foreach (var value in (IEnumerable)record.Value.Value)
                    {
                        if (isFirst)
                        {
                            isFirst = false;
                        }
                        else
                        {
                            writer.Write(", ");
                        }

                        writer.Write(value);
                    }

                    writer.WriteLine();
                }
                else
                {
                    writer.WriteLine("    Value:   {0}", record.Value.Value);
                }
            }
        }
    }
}
