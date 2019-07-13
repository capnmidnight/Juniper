namespace System.IO
{
    public static class FileExt
    {
        /// <summary>
        /// Attempts to delete a file, swallowing any errors along the way.
        /// </summary>
        /// <param name="path">The file to delete.</param>
        /// <returns>
        /// True, if the file was deleted. False, if the file
        /// doesn't exist or it couldn't be deleted (e.g. a process has a file-lock
        /// on it).
        /// </returns>
        public static bool TryDelete(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
                {
                }
#pragma warning restore CA1031 // Do not catch general exception types
            }
            return false;
        }

        /// <summary>
        /// Creates a copy of a file, making sure the destination directory exists.
        /// </summary>
        /// <param name="src">The filepath to copy.</param>
        /// <param name="dest">The filepath to make for the new file.</param>
        /// <param name="overwrite">Set to true to overwrite any files that may already exist in
        /// the destination directory. If false, and the destination file already exists, an error
        /// is thrown.</param>
        public static void Copy(string src, string dest, bool overwrite)
        {
            var destDir = Path.GetDirectoryName(dest);
            DirectoryExt.CreateDirectory(destDir);
            File.Copy(src, dest, overwrite);
        }

        /// <summary>
        /// Writes the given text to a file, ensuring the parent directory for
        /// the file actually exists.
        /// </summary>
        /// <param name="path">The filepath to write.</param>
        /// <param name="text">The content to write to the file.</param>
        public static void WriteAllText(string path, string text)
        {
            var dir = Path.GetDirectoryName(path);
            DirectoryExt.CreateDirectory(dir);
            File.WriteAllText(path, text);
        }
    }
}
