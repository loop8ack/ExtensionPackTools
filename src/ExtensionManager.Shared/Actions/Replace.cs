using System.IO;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes static methods to replace items.
    /// </summary>
    public static class Replace
    {
        /// <summary>
        /// Replaces any existing folder having the fully-qualified
        /// <paramref name="pathname" />
        /// specified, with a folder having the same pathname, but devoid of contained
        /// files and subfolders.
        /// <para />
        /// If the folder with the specified <paramref name="pathname" /> does not already
        /// exist, then this method attempts to create the folder.
        /// </summary>
        /// <param name="pathname">
        /// (Required.) String containing the fully-qualified
        /// pathname of the folder to replace with an empty one.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the operation completed successfully;
        /// <see langword="false" /> otherwise.
        /// </returns>
        /// <remarks>
        /// This method replaces the sequence of calling the
        /// <see cref="M:System.IO.Directory.Delete" /> and
        /// <see cref="M:System.IO.Directory.CreateDirectory" /> methods with a more
        /// robust/fault-tolerant implementation.
        /// <para />
        /// You just never know when an
        /// I/O error is going to crop up during the creation of a directory.
        /// </remarks>
        public static bool Folder(string pathname)
        {
            var result = false;

            // Gotta have an input value
            if (string.IsNullOrWhiteSpace(pathname)) return result;

            try
            {
                result = Remove.Folder(pathname) 
                         && Create.Folder(pathname) 
                         && Directory.Exists(pathname);
            }
            catch
            {
                // Return failed by default.
                result = false;
            }

            return result;
        }
    }
}