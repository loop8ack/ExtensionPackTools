using System.IO;

namespace ExtensionManager
{
    /// <summary>
    /// Exposes static methods to create items.
    /// </summary>
    public static class Create
    {
        /// <summary>
        /// Creates a folder having the fully-qualified <paramref name="pathname" />
        /// specified, if it does not already exist.
        /// <para />
        /// If the folder with the specified <paramref name="pathname" /> exists, then this
        /// method does nothing.
        /// </summary>
        /// <param name="pathname">
        /// (Required.) String containing the fully-qualified
        /// pathname of the folder to create.
        /// </param>
        /// <returns>
        /// <see langword="true" /> if the operation completed successfully;
        /// <see langword="false" /> otherwise.
        /// </returns>
        /// <remarks>
        /// This method replaces the
        /// <see cref="M:System.IO.Directory.CreateDirectory" /> method with a more
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
                // If the folder already exists, simply report success and then quit
                if (Directory.Exists(pathname)) return true;

                Directory.CreateDirectory(pathname);

                // If the operation was successful, then the folder will exist.  Report
                // success or failure on this basis.
                result = Directory.Exists(pathname);
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