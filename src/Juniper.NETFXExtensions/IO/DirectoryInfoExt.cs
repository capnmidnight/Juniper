using System.Collections.Generic;

namespace System.IO
{
    /// <summary>
    /// Extension methods and helper functions for dealing with Directories.
    /// </summary>
    public static class DirectoryInfoExt
    {
        /// <summary>
        /// Recurses through all subdirectories in a given directory to find all
        /// files contained within.
        /// </summary>
        /// <param name="dir">The parent directory to recurse.</param>
        /// <returns>A lazy collection of filenames in both <paramref name="path"/> and all subdirectories of <paramref name="path"/>.</returns>
        public static IEnumerable<FileInfo> RecurseFiles(this DirectoryInfo dir)
        {
            var q = new Queue<DirectoryInfo> { dir };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                q.AddRange(here.GetDirectories());
                foreach (var file in here.GetFiles())
                {
                    yield return file;
                }
            }
        }

        /// <summary>
        /// Recurses through all subdirectories in a given directory to find all
        /// subdirectories contained within.
        /// </summary>
        /// <param name="dir">The parent directory to recurse.</param>
        /// <returns>A lazy collection of directory names in both <paramref name="dir"/> and all subdirectories of <paramref name="dir"/>.</returns>
        public static IEnumerable<DirectoryInfo> RecurseDirectories(this DirectoryInfo dir)
        {
            var q = new Queue<DirectoryInfo> { dir };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                foreach (var subDir in here.GetDirectories())
                {
                    q.Enqueue(subDir);
                    yield return subDir;
                }
            }
        }

        /// <summary>
        /// Recursively deletes all files and subdirectories within a given directory, accumulating a list of
        /// errors along the way.
        /// </summary>
        /// <param name="dir">The directory from which to delete things.</param>
        /// <returns>A list of files that could not be deleted. If there were no errors, the returned value is an empty list.</returns>
        public static List<string> Nuke(this DirectoryInfo dir)
        {
            var allErrors = new List<string>(10);
            foreach (var file in dir.RecurseFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    allErrors.Add(file.FullName);
                }
            }

            if (allErrors.Count == 0)
            {
                dir.Delete(true);
            }

            return allErrors;
        }
    }
}
