using System.IO;

namespace ExtensionManager
{
    /// <summary>
    /// Deletes items, such as folders.
    /// </summary>
    public static class Remove

    {
        /// <summary>
        /// Removes the folder with the specified <paramref name="pathname" />, and
        /// optionally, all its contained files and subfolders, from the disk.
        /// </summary>
        /// <param name="pathname">
        /// (Required.) String containing the fully-qualified
        /// pathname of the folder that is to be removed.
        /// </param>
        /// <param name="recursive">
        /// (Optional.) <see langword="true" /> by default.  If set
        /// to <see langword="true" />, then this method attempts to remove the files and
        /// subfolders contained within the folder having the specified
        /// <paramref name="pathname" />, along with the folder itself.  If set to
        /// <see langword="false" />, then this method will only delete the folder having
        /// the specified <paramref name="pathname" /> in the event that it is currently
        /// empty.
        /// </param>
        /// <returns>
        /// This is a robust, fault-tolerant replacement for the
        /// <see cref="M:System.IO.Directory.Delete" /> method.
        /// <para />
        /// If the <paramref name="recursive" /> parameter is set to
        /// <see langword="false" />, then the method will only work correctly if the
        /// folder having the specified <paramref name="pathname" /> is currently empty.
        /// </returns>
        public static bool Folder(string pathname, bool recursive = true)
        {
            var result = false;

            if (string.IsNullOrWhiteSpace(pathname)) return result;

            try
            {
                if (!Directory.Exists(pathname))
                    return true; // nothing to do; return success

                Directory.Delete(pathname, recursive);
            }
            catch
            {
                result = false;
            }

            return result;
        }
    }
}