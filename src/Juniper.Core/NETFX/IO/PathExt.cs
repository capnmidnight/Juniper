using System.Linq;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// A static class containing a few functions that the normal System.IO.Path class does not contain.
    /// </summary>
    public static class PathExt
    {
        /// <summary>
        /// Converts paths specified in any combination of forward- or back-slashes to the correct
        /// slash for the current system.
        /// </summary>
        /// <param name="path">The path to fix.</param>
        /// <returns>The same path, but with the wrong slashes replaced with the right slashes.</returns>
        /// <example>On Windows, if a path is specified with forward slashes, a value with backslashes is returned.</example>
        public static string FixPath(string path)
        {
            return FixPath(FixPath(path, '\\'), '/');
        }

        private static string FixPath(string path, char sep)
        {
            if (sep == Path.DirectorySeparatorChar)
            {
                return path;
            }
            else
            {
                string prefix = string.Empty;
                var parts = path.SplitX(sep).ToList();
                if (parts.Count > 0 && parts[0].EndsWith(":"))
                {
                    prefix = parts[0] + Path.DirectorySeparatorChar;
                    parts.RemoveAt(0);
                }
                return prefix + Path.Combine(parts.ToArray());
            }
        }

        /// <summary>
        /// Creates a file path that is relative to the currently-edited demo path.
        /// </summary>
        /// <returns>The relative path.</returns>
        /// <param name="fullPath">Full path.</param>
        /// <param name="directory">
        /// The directory from which to consider the relative path. If no value is provided (i.e.
        /// `null` or empty string), then the current working directory is used.
        /// </param>
        public static string Abs2Rel(string fullPath, string directory = null)
        {
            if (!Path.IsPathRooted(fullPath))
            {
                return fullPath;
            }
            else
            {
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Environment.CurrentDirectory;
                }

                var partsA = FixPath(directory).SplitX(Path.DirectorySeparatorChar).ToList();
                var partsB = FixPath(fullPath).SplitX(Path.DirectorySeparatorChar).ToList();

                while (partsA.Count > 0
                       && partsB.Count > 0
                       && partsA[0] == partsB[0])
                {
                    partsA.RemoveAt(0);
                    partsB.RemoveAt(0);
                }

                if (partsB.Count == 0)
                {
                    return null;
                }
                else
                {
                    return Path
                        .Combine(partsA
                        .Select(_ => "..")
                        .Concat(partsB)
                        .ToArray());
                }
            }
        }

        /// <summary>
        /// Resolves an absolute path from a path that is relative to the currently-edited demo path.
        /// </summary>
        /// <returns>The absolute path.</returns>
        /// <param name="relativePath">Relative path.</param>
        /// <param name="directory">
        /// The directory from which to consider the relative path. If no value is provided (i.e.
        /// `null` or empty string), then the current working directory is used.
        /// </param>
        public static string Rel2Abs(string relativePath, string directory = null)
        {
            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }
            else
            {
                if (string.IsNullOrEmpty(directory))
                {
                    directory = Environment.CurrentDirectory;
                }

                var partsA = FixPath(directory).SplitX(Path.DirectorySeparatorChar).ToList();
                var partsB = FixPath(relativePath).SplitX(Path.DirectorySeparatorChar).ToList();

                while (partsA.Count > 0
                       && partsB.Count > 0
                       && partsB[0] == "..")
                {
                    partsA.RemoveAt(partsA.Count - 1);
                    partsB.RemoveAt(0);
                }

                if (partsB.Count == 0)
                {
                    return null;
                }
                else
                {
                    var parts = partsA
                        .Concat(partsB)
                        .ToArray();

                    // Handle Windows drive letters
                    if (Path.VolumeSeparatorChar == ':' && parts[0].Last() == Path.VolumeSeparatorChar)
                    {
                        parts[0] += Path.DirectorySeparatorChar;
                        parts[0] += Path.DirectorySeparatorChar;
                    }

                    return Path.Combine(parts);
                }
            }
        }

        public static string RemoveInvalidChars(this string filePath)
        {
            return string.Join("_", filePath.Split(Path.GetInvalidFileNameChars()));
        }
    }
}