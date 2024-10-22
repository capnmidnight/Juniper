namespace System.IO;



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
        ArgumentNullException.ThrowIfNull(dir);

        dir = new DirectoryInfo(Path.Combine(subs.Prepend(dir.FullName).ToArray()));

        if (!dir.Exists && create)
        {
            dir.Create();
        }

        return dir;
    }

    public static DirectoryInfo CD(this DirectoryInfo dir, params string[] subs) =>
        dir.DirectoryOp(subs, false);

    public static DirectoryInfo? GoUpUntil(this DirectoryInfo dir, Func<DirectoryInfo, bool> test)
    {
        var here = dir;
        while (here is not null && !test(here))
        {
            here = here.Parent;
        }

        return here;
    }

    /// <summary>
    /// Retrieve the named subdirectory of a given directory, creating it if it doesn't exist.
    /// </summary>
    /// <param name="sub">The sub.</param>
    /// <returns></returns>
    public static DirectoryInfo MkDir(this DirectoryInfo dir, params string[] subs) =>
        dir.DirectoryOp(subs, true);

    /// <summary>
    /// Makes a file info reference in a given directory.
    /// </summary>
    /// <param name="fileName">The name of the file to "touch".</param>
    /// <returns></returns>
    public static FileInfo Touch(this DirectoryInfo dir, string fileName)
    {
        ArgumentNullException.ThrowIfNull(dir);

        var file = new FileInfo(PathExt.Combine(dir.FullName, fileName));
        return file;
    }

    public static string? MaybeReadText(this FileInfo file)
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
        ArgumentNullException.ThrowIfNull(dir);

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
        if (file?.Directory is null)
        {
            return false;
        }

        return Contains(dir, file.Directory);
    }

    public static bool Contains(this DirectoryInfo dir, DirectoryInfo subDir)
    {
        ArgumentNullException.ThrowIfNull(dir);
        ArgumentNullException.ThrowIfNull(subDir);

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

    private static readonly EnumerationOptions enumerationOptions = new()
    {
        IgnoreInaccessible = true,
        MatchType = MatchType.Simple,
        AttributesToSkip = FileAttributes.Archive
            | FileAttributes.Hidden
            | FileAttributes.Encrypted
            | FileAttributes.Compressed
            | FileAttributes.Offline
    };

    public static IEnumerable<DirectoryInfo> LazyRecurse(this DirectoryInfo directory, Func<DirectoryInfo, bool>? filter = null)
    {
        var queue = new Queue<DirectoryInfo>{
            directory
        };

        while (queue.Count > 0)
        {
            var here = queue.Dequeue();
            if (here.Exists && filter?.Invoke(here) != false)
            {
                var errored = false;
                try { queue.AddRange(here.GetDirectories("*", enumerationOptions)); }
                catch { errored = true; }
                if (!errored)
                {
                    yield return here;
                }
            }
        }
    }

    public static IEnumerable<string> LazyRecurse(string directory, Func<string, bool>? filter = null) =>
        from dir in new DirectoryInfo(directory)
            .LazyRecurse(filter is not null
                ? dir => filter(dir.FullName)
                : null)
        select dir.FullName;

    public static bool TryDelete(this DirectoryInfo directory, bool recursive = false)
    {
        if (directory?.Exists ?? false)
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
}