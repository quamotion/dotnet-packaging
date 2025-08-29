using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using Xunit;
using Packaging.Targets;
using Packaging.Targets.Deb;
using Packaging.Targets.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Security.Cryptography;

namespace Packaging.Targets.Tests.Deb
{
    public class DebTaskIntegrationTests : IDisposable
    {
        private readonly string tempDirectory;
        private readonly string publishDir;
        private readonly string outputDir;

        public DebTaskIntegrationTests()
        {
            // Create a temporary directory that will be cleaned up automatically
            tempDirectory = Path.Combine(Path.GetTempPath(), "DebTaskTest_" + Guid.NewGuid().ToString("N")[..8]);
            Directory.CreateDirectory(tempDirectory);

            publishDir = Path.Combine(tempDirectory, "publish");
            outputDir = Path.Combine(tempDirectory, "output");

            Directory.CreateDirectory(publishDir);
            Directory.CreateDirectory(outputDir);
        }

        public void Dispose()
        {
            // Cleanup temporary directory
            if (Directory.Exists(tempDirectory))
            {
                try
                {
                    Directory.Delete(tempDirectory, recursive: true);
                }
                catch
                {
                    // Best effort cleanup - don't fail test if cleanup fails
                }
            }
        }

        [Fact]
        public void CreateAndVerifyRealDebPackage()
        {
            // Fake .deb contents to validate against
            var testFiles = new Dictionary<string, string>
            {
                ["usr/bin/myapp"] = "#!/bin/bash\necho 'Hello from myapp'\nexit 0\n",
                ["usr/share/myapp/config.json"] = "{\"name\": \"myapp\", \"version\": \"1.0.0\", \"enabled\": true}",
                ["usr/share/myapp/data.txt"] = "This is some test data\nLine 2\nLine 3\nEnd of file\n",
                ["etc/myapp/settings.conf"] = "# Configuration file\nlog_level=INFO\nport=8080\n",
                ["usr/share/doc/myapp/README.md"] = "# MyApp\n\nThis is a test application for .deb packaging.\n\n## Usage\n\nRun with `myapp`.\n",
                ["usr/share/myapp/large_content_file1.bin"] = getCompressableString(1024 * 1024 * 100),
                ["usr/share/myapp/large_content_file2.bin"] = getCompressableString(1024 * 1024 * 100),
            };

            CreateTestFiles(testFiles);

            // Create Content items for DebTask (this is what MSBuild would normally provide)
            var contentItems = CreateContentItems(testFiles);

            var debPath = Path.Combine(outputDir, "myapp.deb");
            var debTarPath = Path.Combine(outputDir, "myapp.tar");
            var debTarXzPath = Path.Combine(outputDir, "myapp.tar.xz");

            var debTask = new DebTask
            {
                PublishDir = publishDir,
                DebPath = debPath,
                DebTarPath = debTarPath,
                DebTarXzPath = debTarXzPath,
                Prefix = "/",
                Version = "1.0.0-test",
                PackageName = "myapp",
                Content = contentItems,
                Maintainer = "Test Suite <test@example.com>",
                Description = "Test package for DebTask integration test",
                RuntimeIdentifier = "linux-x64",
                DebPackageArchitecture = "amd64",
                Section = "utils",
                Homepage = "https://example.com/myapp",
                Priority = "optional",
                BuildEngine = new MockBuildEngine()
            };

            Assert.True(debTask.Execute(), "DebTask.Execute() should succeed");
            Assert.True(File.Exists(debPath), $"DEB file should exist at {debPath}");
            Assert.True(File.Exists(debTarPath), $"TAR file should exist at {debTarPath}");
            Assert.True(File.Exists(debTarXzPath), $"TAR.XZ file should exist at {debTarXzPath}");

            var debInfo = new FileInfo(debPath);
            Assert.True(debInfo.Length > 1000, "DEB file should have substantial content");

            // Verify XZ compression worked (compressed file should be smaller than TAR)
            var tarInfo = new FileInfo(debTarPath);
            var tarXzInfo = new FileInfo(debTarXzPath);
            Assert.True(tarXzInfo.Length < tarInfo.Length, "XZ compressed file should be smaller than TAR");
            Assert.True(tarXzInfo.Length > 0, "XZ compressed file should have content");

            // Read the .deb package back and verify we can extract the payload
            using var debStream = File.OpenRead(debPath);
            var package = DebPackageReader.Read(debStream);
            Assert.NotNull(package);

            // Extract and verify the compressed payload can be read
            var extractedFiles = ExtractDebPayload(debPath);
            foreach (var kvp in testFiles)
            {
                // TAR paths can have ./path or .//path format - find the file by checking both variants
                var expectedPath1 = "./" + kvp.Key;
                var expectedPath2 = ".//" + kvp.Key;

                string actualPath = null;
                if (extractedFiles.ContainsKey(expectedPath1))
                {
                    actualPath = expectedPath1;
                }
                else if (extractedFiles.ContainsKey(expectedPath2))
                {
                    actualPath = expectedPath2;
                }

                Assert.True(actualPath != null, $"File {expectedPath1} or {expectedPath2} should be in the DEB payload. Found files: {string.Join(", ", extractedFiles.Keys)}");
                Assert.Equal(kvp.Value, extractedFiles[actualPath]);
            }

            Assert.True(extractedFiles.Count >= testFiles.Count,
                $"Should have at least {testFiles.Count} files, but found {extractedFiles.Count}");
        }

        private void CreateTestFiles(Dictionary<string, string> testFiles)
        {
            foreach (var kvp in testFiles)
            {
                var fullPath = Path.Combine(publishDir, kvp.Key);
                var directory = Path.GetDirectoryName(fullPath);

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(fullPath, kvp.Value, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
            }
        }

        private string getCompressableString(int length) {
            var bytes = new byte[length / 4];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            var data =  Convert.ToBase64String(bytes);
            data += new string('a', length/4);
            data += new string('b', length/4);
            data += new string('c', length/4);
            return data;
        }

        private void CreateTarFile(string sourceDir, string tarPath)
        {
            var entries = new List<ArchiveEntry>();
            CreateArchiveEntries(sourceDir, "", entries);

            using var tarStream = File.Create(tarPath);
            TarFileCreator.FromArchiveEntries(entries, tarStream);
        }

        private void CreateArchiveEntries(string baseDir, string relativePath, List<ArchiveEntry> entries)
        {
            var currentDir = Path.Combine(baseDir, relativePath);

            foreach (var file in Directory.GetFiles(currentDir))
            {
                var fileName = Path.GetFileName(file);
                var tarPath = string.IsNullOrEmpty(relativePath) ? $"/{fileName}" : $"/{relativePath}/{fileName}";

                var fileInfo = new FileInfo(file);
                var entry = new ArchiveEntry
                {
                    TargetPath = tarPath,
                    SourceFilename = file,
                    FileSize = (uint)fileInfo.Length,
                    Mode = LinuxFileMode.S_IRUSR | LinuxFileMode.S_IWUSR | LinuxFileMode.S_IRGRP | LinuxFileMode.S_IROTH, // 644
                    Modified = fileInfo.LastWriteTimeUtc,
                    Owner = "root",
                    Group = "root"
                };

                entries.Add(entry);
            }

            foreach (var dir in Directory.GetDirectories(currentDir))
            {
                var dirName = Path.GetFileName(dir);
                var newRelativePath = string.IsNullOrEmpty(relativePath) ? dirName : $"{relativePath}/{dirName}";

                CreateArchiveEntries(baseDir, newRelativePath, entries);
            }
        }

        private Dictionary<string, string> ExtractTarPayload(Stream tarStream)
        {
            var files = new Dictionary<string, string>();

            using var tarFile = new TarFile(tarStream, leaveOpen: true);

            while (tarFile.Read())
            {
                // Skip directories and other non-regular files - check if we can read the file
                if (!string.IsNullOrEmpty(tarFile.FileName) && ((TarHeader)tarFile.FileHeader).FileSize > 0)
                {
                    try
                    {
                        using var entryStream = tarFile.Open();
                        using var memoryStream = new MemoryStream();
                        entryStream.CopyTo(memoryStream);

                        var content = Encoding.UTF8.GetString(memoryStream.ToArray());
                        files[tarFile.FileName] = content;
                    }
                    catch
                    {
                        // Skip files that can't be read (might be directories or special files)
                    }
                }
            }

            return files;
        }

        private ITaskItem[] CreateContentItems(Dictionary<string, string> testFiles)
        {
            var items = new List<ITaskItem>();

            foreach (var kvp in testFiles)
            {
                var item = new TaskItem(Path.Combine(publishDir, kvp.Key));
                item.SetMetadata("TargetPath", kvp.Key);
                items.Add(item);
            }

            return items.ToArray();
        }

        private Dictionary<string, string> ExtractDebPayload(string debPath)
        {
            using var debStream = File.OpenRead(debPath);

            // Get the decompressed payload stream (data.tar)
            using var payloadStream = DebPackageReader.GetPayloadStream(debStream);

            // Extract TAR contents
            return ExtractTarPayload(payloadStream);
        }
    }

    internal class MockBuildEngine : IBuildEngine
    {
        public bool ContinueOnError => false;
        public int LineNumberOfTaskNode => 0;
        public int ColumnNumberOfTaskNode => 0;
        public string ProjectFileOfTaskNode => "";

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            return true;
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
        }
    }
}
