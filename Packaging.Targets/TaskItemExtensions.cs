using Microsoft.Build.Framework;
using System;
using System.IO;
using System.Linq;

namespace Packaging.Targets
{
    /// <summary>
    /// Provides extension methods for the <see cref="ITaskItem"/> interface.
    /// </summary>
    public static class TaskItemExtensions
    {
        /// <summary>
        /// Gets a value indicating whether this item is copied to the publish directory or not.
        /// </summary>
        /// <param name="item">
        /// The item for which to determine whether it is copied or not.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the file is copied over; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool IsPublished(this ITaskItem item)
        {
            if (item == null)
            {
                return false;
            }

            if (!item.MetadataNames.OfType<string>().Contains("CopyToPublishDirectory"))
            {
                return false;
            }

            var copyToPublishDirectoryValue = item.GetMetadata("CopyToPublishDirectory");
            CopyToDirectoryValue copyToPublishDirectory;

            if (!Enum.TryParse<CopyToDirectoryValue>(copyToPublishDirectoryValue, out copyToPublishDirectory))
            {
                return false;
            }

            return copyToPublishDirectory != CopyToDirectoryValue.DoNotCopy;
        }

        /// <summary>
        /// Gets the path to where the file is published.
        /// </summary>
        /// <param name="item">
        /// The item for which to determine the publish path.
        /// </param>
        /// <returns>
        /// The path to where the file is published.
        /// </returns>
        public static string GetPublishedPath(this ITaskItem item)
        {
            if (item == null)
            {
                return null;
            }

            var link = item.GetMetadata("Link");
            if (!string.IsNullOrEmpty(link))
            {
                return link.Replace("\\", "/");
            }

            var relativeDirectory = item.GetMetadata("RelativeDir");
            var filename = item.GetMetadata("FileName");
            var extension = item.GetMetadata("Extension");

            return Path.Combine(relativeDirectory, $"{filename}{extension}").Replace("\\", "/");
        }

        /// <summary>
        /// Gets the path of the file in the Linux filesystem.
        /// </summary>
        /// <param name="item">
        /// The item for which to get the Linux path.
        /// </param>
        /// <returns>
        /// The path to the file on the Linux filesystem.
        /// </returns>
        public static string GetLinuxPath(this ITaskItem item)
        {
            return TryGetValue(item, "LinuxPath");
        }

        /// <summary>
        /// Gets the file mode of the file in the Linux filesystem.
        /// </summary>
        /// <param name="item">
        /// The item for which to get the file mode.
        /// </param>
        /// <returns>
        /// The file mode of the file on the Linux file system.
        /// </returns>
        public static string GetLinuxFileMode(this ITaskItem item)
        {
            return TryGetValue(item, "LinuxFileMode");
        }

        /// <summary>
        /// Gets the Linux owner of the file.
        /// </summary>
        /// <param name="item">
        /// The item for which to get the file owner.
        /// </param>
        /// <returns>
        /// The owner of the file.
        /// </returns>
        public static string GetOwner(this ITaskItem item)
        {
            return TryGetValue(item, "Owner", "root");
        }

        /// <summary>
        /// Gets the Linux group of the file.
        /// </summary>
        /// <param name="item">
        /// The item for which to get the file group.
        /// </param>
        /// <returns>
        /// The group of the file.
        /// </returns>
        public static string GetGroup(this ITaskItem item)
        {
            return TryGetValue(item, "Group", "root");
        }

        /// <summary>
        /// Gets the version of the RPM dependency.
        /// </summary>
        /// <param name="item">
        /// The task item which represents the RPM dependency.
        /// </param>
        /// <returns>
        /// The version of the dependency.
        /// </returns>
        public static string GetVersion(this ITaskItem item)
        {
            return TryGetValue(item, "Version", null);
        }

        /// <summary>
        /// Gets a value indicating whether the item should be removed when the
        /// program is removed.
        /// </summary>
        /// <param name="item">
        /// The item to inspect.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the file should be removed; otherwise,
        /// <see langword="false"/>.
        /// </returns>
        public static bool GetRemoveOnUninstall(this ITaskItem item)
        {
            var valueString = TryGetValue(item, "RemoveOnUninstall", "false");
            bool value;

            if (!bool.TryParse(valueString, out value))
            {
                return false;
            }

            return value;
        }

        private static string TryGetValue(ITaskItem item, string name, string @default = null)
        {
            if (item == null)
            {
                return @default;
            }

            if (!item.MetadataNames.OfType<string>().Contains(name))
            {
                return @default;
            }

            var linuxPath = item.GetMetadata(name);

            return linuxPath;
        }
    }
}
