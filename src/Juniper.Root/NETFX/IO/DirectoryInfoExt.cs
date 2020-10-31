using System.Collections.Generic;
using System.Linq;

namespace System.IO
{


    /// <summary>
    /// Extension methods and helper functions for dealing with Directories.
    /// </summary>
    public static class DirectoryInfoExt
    {
        /// <summary>
        /// Retrieve the named subdirectory of a given directory, or Null if it doesn't exist.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        public static DirectoryInfo CD(this DirectoryInfo dir, string sub)
        {
            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            return dir.GetDirectories()
                .FirstOrDefault(d => d.Name == sub);
        }

        /// <summary>
        /// Retrieve the named subdirectory of a given directory, or Null if it doesn't exist.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        public static DirectoryInfo MkDir(this DirectoryInfo dir, string sub)
        {
            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            var subdir = new DirectoryInfo(Path.Combine(dir.FullName, sub));
            subdir.Create();
            return subdir;
        }

        /// <summary>
        /// Makes a file info reference in a given directory.
        /// </summary>
        /// <param name="fileName">The name of the file to "touch".</param>
        /// <returns></returns>
        public static FileInfo Touch(this DirectoryInfo dir, string fileName)
        {
            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            var file = new FileInfo(Path.Combine(dir.FullName, fileName));
            return file;
        }

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
            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            var allErrors = new List<string>(10);
            foreach (var file in dir.RecurseFiles())
            {
                try
                {
                    file.Delete();
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                    allErrors.Add(file.FullName);
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }

            if (allErrors.Count == 0)
            {
                dir.Delete(true);
            }

            return allErrors;
        }

        public static bool Contains(this DirectoryInfo dir, FileInfo file)
        {
            if (file is null)
            {
                return false;
            }

            return Contains(dir, file.Directory);
        }

        public static bool Contains(this DirectoryInfo dir, DirectoryInfo subDir)
        {
            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            if (subDir is null)
            {
                throw new ArgumentNullException(nameof(subDir));
            }

            var subDirParts = subDir.FullName.SplitX(Path.DirectorySeparatorChar);
            var dirParts = dir.FullName.SplitX(Path.DirectorySeparatorChar);
            if (subDirParts.Length < dirParts.Length)
            {
                return false;
            }
            else
            {
                return dirParts.Matches(subDirParts.Take(dirParts.Length));
            }
        }

        public static bool IsJunction(this DirectoryInfo directory)
        {
            return directory is object
                && (directory.Attributes & FileAttributes.ReparsePoint) != 0;
        }
    }
}