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
        private static DirectoryInfo DirectoryOp(this DirectoryInfo dir, string[] subs, bool create)
        {
            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            dir = new DirectoryInfo(Path.Combine(subs.Prepend(dir.FullName).ToArray()));

            if (!dir.Exists && create)
            {
                dir.Create();
            }

            return dir;
        }

        public static DirectoryInfo CD(this DirectoryInfo dir, params string[] subs)
        {
            return dir.DirectoryOp(subs, false);
        }

        /// <summary>
        /// Retrieve the named subdirectory of a given directory, creating it if it doesn't exist.
        /// </summary>
        /// <param name="sub">The sub.</param>
        /// <returns></returns>
        public static DirectoryInfo MkDir(this DirectoryInfo dir, params string[] subs)
        {
            return dir.DirectoryOp(subs, true);
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

        public static string MaybeReadText(this FileInfo file)
        {
            if (!file.Exists)
            {
                return null;
            }

            return File.ReadAllText(file.FullName);
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
            foreach (var file in dir.EnumerateFiles("*", SearchOption.AllDirectories))
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
            return directory is not null
                && (directory.Attributes & FileAttributes.ReparsePoint) != 0;
        }

        public static FileInfo TouchCopy(this FileInfo file, DirectoryInfo dest)
        {
            return dest.Touch(file.Name);
        }

        public static Dictionary<string, (FileInfo From, FileInfo To)> CopyFiles(this DirectoryInfo src, DirectoryInfo dest) {
            return src.GetFiles()
                .ToDictionary(
                    f => f.Name,
                    f => (f, f.TouchCopy(dest))
                );
        }
    }
}