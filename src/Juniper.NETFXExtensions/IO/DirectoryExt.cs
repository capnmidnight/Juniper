using System.Collections.Generic;

namespace System.IO
{
    /// <summary>
    /// Extension methods and helper functions for dealing with Directories.
    /// </summary>
    public static class DirectoryExt
    {
        /// <summary>
        /// Recurses through all subdirectories in a given directory to find all
        /// files contained within.
        /// </summary>
        /// <param name="path">The parent directory to recurse.</param>
        /// <returns>A lazy collection of filenames in both <paramref name="path"/> and all subdirectories of <paramref name="path"/>.</returns>
        public static IEnumerable<string> RecurseFiles(string path)
        {
            var q = new Queue<string>() { path };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                q.AddRange(Directory.GetDirectories(here));
                foreach (var file in Directory.GetFiles(here))
                {
                    yield return file;
                }
            }
        }

        /// <summary>
        /// Recurses through all subdirectories in a given directory to find all
        /// subdirectories contained within.
        /// </summary>
        /// <param name="path">The parent directory to recurse.</param>
        /// <returns>A lazy collection of directory names in both <paramref name="path"/> and all subdirectories of <paramref name="path"/>.</returns>
        public static IEnumerable<string> RecurseDirectories(string path)
        {
            var q = new Queue<string>() { path };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                yield return here;
                q.AddRange(Directory.GetDirectories(here));
            }
        }

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
                {
                }
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
