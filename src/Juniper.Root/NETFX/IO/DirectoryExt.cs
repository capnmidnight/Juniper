namespace System.IO
{
    /// <summary>
    /// Extension methods and helper functions for dealing with Directories.
    /// </summary>
    public static class DirectoryExt
    {
        /// <summary>
        /// Attempts to delete a directory, swallowing any errors along the way.
        /// </summary>
        /// <param name="path">The directory to delete.</param>
        /// <returns>
        /// True, if the directory was deleted. False, if the directory
        /// doesn't exist or it couldn't be deleted (e.g. a process has a file-lock
        /// on a file within the directory).
        /// </returns>
        public static bool TryDelete(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                    return true;
                }
                catch
                { }
            }

            return false;
        }

        public static bool TryDelete(this DirectoryInfo directory, bool recursive = false)
        {
            if (directory?.Exists == true)
            {
                try
                {
                    directory.Delete(recursive);
                    return true;
                }
                catch
                { }
            }

            return false;
        }

        /// <summary>
        /// Creates a directory, if the directory is semi-valid.
        /// </summary>
        /// <param name="dir">The directory name to create. This could be an
        /// absolute directory path or a path relative to the current working
        /// directory.</param>
        public static void CreateDirectory(string dir)
        {
            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}