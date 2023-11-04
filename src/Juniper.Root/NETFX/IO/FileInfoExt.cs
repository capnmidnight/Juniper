using System.Text;

namespace System.IO
{
    /// <summary>
    /// Extension methods and helper functions for dealing with Files.
    /// </summary>
    public static class FileInfoExt
    {
        /// <summary>
        /// Copies a file to a directory.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="dir">The dir.</param>
        /// <param name="skipOverwrite">Whether or not overwriting should be skipped. Defaults to false.</param>
        public static FileInfo CopyTo(this FileInfo file, DirectoryInfo dir, bool skipOverwrite = false)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }

            if (!dir.Exists)
            {
                dir.Create();
            }

            var outputFileName = Path.Combine(dir.FullName, file.Name);
            if (!skipOverwrite || !File.Exists(outputFileName))
            {
                file.CopyTo(outputFileName);
            }

            return new FileInfo(outputFileName);
        }

        /// <summary>
        /// Gets a file path for the given file, relative to a given directory.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="dir">The dir.</param>
        /// <returns></returns>
        public static string? RelativeTo(this FileInfo file, DirectoryInfo dir)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (dir is null)
            {
                throw new ArgumentNullException(nameof(dir));
            }

            return PathExt.Abs2Rel(file.FullName, dir.FullName);
        }

        public static string GetShortName(this FileInfo file) =>
            Path.Combine(file.Directory?.Name ?? "", file.Name);

        public static string? QuotePath(this string? path)
        {
            if (path is not null
                && path.Contains(' '))
            {
                var sb = new StringBuilder(path);
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    if (path[0] != '"' || path[^1] != '"')
                    {
                        sb.Insert(0, '"');
                        sb.Append('"');
                    }
                }
                else
                {
                    for (var i = sb.Length - 1; i >= 0; i--)
                    {
                        if (sb[i] == ' '
                            && (i == 0
                                || sb[i - 1] != '\\'))
                        {
                            sb.Insert(i, '\\');
                        }
                    }
                }
                path = sb.ToString();
            }

            return path;
        }
    }
}