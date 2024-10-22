namespace Juniper.TSBuild;

/// <summary>
/// An encapsulation of a file copy operation.
/// </summary>
public struct BuildSystemDependency
{
    /// <summary>
    /// A string that will appear in the build output log
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The file that will be copied
    /// </summary>
    public FileInfo From { get; set; }

    /// <summary>
    /// The file that will be created
    /// </summary>
    public FileInfo To { get; set; }
    
    /// <summary>
    /// Creates an encapsulation of a file copy operation
    /// </summary>
    /// <param name="Name">A string that will appear in the build output log</param>
    /// <param name="From">The file to be copied</param>
    /// <param name="To">The file to be created</param>
    public readonly void Deconstruct(out string Name, out FileInfo From, out FileInfo To)
    {
        Name = this.Name;
        From = this.From;
        To = this.To;
    }
}

public static class FileInfoExtensions
{
    /// <summary>
    /// Create a <see cref="BuildSystemDependency"/> for copying all of the
    /// files of one directory to another.
    /// </summary>
    /// <param name="src">Where the files to be copied are stored</param>
    /// <param name="dest">Where the files are to be copied</param>
    /// <param name="searchOption">One of the enumeration values that specifies
    /// whether the search operation should include only the current directory
    /// or all subdirectories.</param>
    /// <returns></returns>
    public static IEnumerable<BuildSystemDependency> CopyFiles(this DirectoryInfo src, DirectoryInfo dest, SearchOption searchOption, Func<FileInfo, bool>? predicate = null) =>
        src.CopyFiles(dest, "*", searchOption, predicate);

    /// <summary>
    /// Create a <see cref="BuildSystemDependency"/> for copying all of the
    /// files of one directory to another.
    /// </summary>
    /// <param name="src">Where the files to be copied are stored</param>
    /// <param name="dest">Where the files are to be copied</param>
    /// <param name="searchPattern">The search string to match against the 
    /// names of files. This parameter can contain a combination of valid
    /// literal path and wildcard (* and ?) characters, but it doesn't support
    /// regular expressions.</param>
    /// <param name="searchOption">One of the enumeration values that specifies
    /// whether the search operation should include only the current directory
    /// or all subdirectories.</param>
    /// <returns></returns>
    public static IEnumerable<BuildSystemDependency> CopyFiles(this DirectoryInfo src, DirectoryInfo dest, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly, Func<FileInfo, bool>? predicate = null) =>
        src.GetFiles(searchPattern, searchOption)
            .Where(predicate ?? (f => true))
            .Select(f =>
            {
                var d = dest;
                if (f.Directory is not null && f.Directory.FullName != src.FullName)
                {
                    // make sure the shape of the output path matches the shape
                    // of the input path.
                    var parts = f.Directory.FullName
                        .Replace(src.FullName, "")
                        .Split(Path.DirectorySeparatorChar)
                        // skip the blank entry that comes about from the
                        // leading slash
                        .Skip(1)
                        .ToArray();
                    d = d.CD(parts);
                }
                return f.CopyFile(d);
            });

    /// <summary>
    /// Create a <see cref="BuildSystemDependency"/> for copying a single
    /// file to another directory, without changing the file's name.
    /// </summary>
    /// <param name="from">The file to copy</param>
    /// <param name="to">The directory to which to copy the file</param>
    /// <returns></returns>
    public static BuildSystemDependency CopyFile(this FileInfo from, DirectoryInfo to) =>
        from.CopyFile(to.Touch(from.Name));

    /// <summary>
    /// Create <see cref="BuildSystemDependency"/>s for copying multiple
    /// files to another directory, without changing the files' names.
    /// </summary>
    /// <param name="from">The files to copy</param>
    /// <param name="to">The directory to which to copy the files</param>
    /// <returns></returns>
    public static IEnumerable<BuildSystemDependency> CopyFiles(this IEnumerable<FileInfo> froms, DirectoryInfo to) =>
        froms.Select(from => from.CopyFile(to.Touch(from.Name)));


    /// <summary>
    /// Create a <see cref="BuildSystemDependency"/> for copying a single
    /// file to another directory, changing it's name in the process.
    /// </summary>
    /// <param name="from">The file to copy</param>
    /// <param name="to">The directory to which to copy the file</param>
    /// <returns></returns>
    public static BuildSystemDependency CopyFile(this FileInfo from, FileInfo to) => 
        new ()
        {
            Name = $"{to.Directory?.Name}/{from.Name}",
            From = from,
            To = to
        };
}
