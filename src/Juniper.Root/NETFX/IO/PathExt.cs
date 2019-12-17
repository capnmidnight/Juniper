using System.Linq;
using System.Text;

namespace System.IO
{
    /// <summary>
    /// A static class containing a few functions that the normal System.IO.Path class does not contain.
    /// </summary>
    public static class PathExt
    {
        private static readonly char[] INVALID_CHARS = Path.GetInvalidFileNameChars()
            .Where(c => c != Path.DirectorySeparatorChar)
            .ToArray();

        private static readonly char[] INVALID_START_CHARS = INVALID_CHARS
            .Where(c => c != ':')
            .ToArray();

        /// <summary>
        /// Converts paths specified in any combination of forward- or back-slashes to the correct
        /// slash for the current system.
        /// </summary>
        /// <param name="path">The path to fix.</param>
        /// <returns>The same path, but with the wrong slashes replaced with the right slashes.</returns>
        /// <example>On Windows, if a path is specified with forward slashes, a value with backslashes is returned.</example>
        public static string FixPath(string path)
        {
            path = path.NormalizePath();

            var parts = path.SplitX(Path.DirectorySeparatorChar);
            var sb = new StringBuilder();
            for (var i = 0; i < parts.Length; ++i)
            {
                if (i > 0)
                {
                    sb.Append(Path.DirectorySeparatorChar);
                }

                var subParts = i == 0
                    ? parts[i].Split(INVALID_START_CHARS)
                    : parts[i].Split(INVALID_CHARS);

                for (var j = 0; j < subParts.Length; ++j)
                {
                    if (j > 0)
                    {
                        sb.Append('_');
                    }

                    sb.Append(subParts[j]);
                }
            }

            return sb.ToString();
        }

        public static string[] PathParts(string path)
        {
            return FixPath(path).SplitX(Path.DirectorySeparatorChar);
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
        public static string Abs2Rel(string fullPath, string directory)
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

                var partsA = PathParts(directory);
                var partsB = PathParts(fullPath);

                var counter = 0;
                while (counter < partsA.Length
                       && counter < partsB.Length
                       && partsA[counter] == partsB[counter])
                {
                    ++counter;
                }

                if (counter == partsB.Length - 1)
                {
                    return null;
                }
                else
                {
                    var aLen = partsA.Length - counter;
                    var bLen = partsB.Length - counter;
                    var parts = new string[aLen + bLen];
                    for (var i = 0; i < aLen; ++i)
                    {
                        parts[i] = "..";
                    }

                    Array.Copy(partsB, counter, parts, aLen, bLen);

                    return Path.Combine(parts);
                }
            }
        }

        public static string GetLongExtension(string path)
        {
            var i = path.IndexOf('.');
            if (i <= 0)
            {
                return null;
            }
            else
            {
                return path.Substring(i + 1);
            }
        }

        public static string GetShortExtension(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            var i = path.LastIndexOf('.');
            if (i <= 0)
            {
                return null;
            }
            else
            {
                return path.Substring(i + 1);
            }
        }

        private static string Remove(string name, string ext)
        {
            if (ext != null)
            {
                name = name.Substring(0, name.Length - ext.Length - 1);
            }
            return name;
        }

        public static string RemoveShortExtension(string name)
        {
            return Remove(name, GetShortExtension(name));
        }

        public static string RemoveLongExtension(string name)
        {
            return Remove(name, GetLongExtension(name));
        }

        public static string Abs2Rel(string fullPath)
        {
            return Abs2Rel(fullPath, null);
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
        public static string Rel2Abs(string relativePath, string directory)
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

                var partsA = PathParts(directory).ToList();
                var partsB = PathParts(relativePath).ToList();

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

        public static string Rel2Abs(string relativePath)
        {
            return Rel2Abs(relativePath, null);
        }
    }
}