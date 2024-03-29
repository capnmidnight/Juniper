﻿namespace Juniper.TSBuild;

public struct BuildSystemDependency
{
    public string Name { get; set; }
    public FileInfo From { get; set; }
    public FileInfo To { get; set; }
    
    public readonly void Deconstruct(out string Name, out FileInfo From, out FileInfo To)
    {
        Name = this.Name;
        From = this.From;
        To = this.To;
    }
}

public static class FileInfoExtensions
{
    public static FileInfo TouchCopy(this FileInfo file, DirectoryInfo dest) =>
        dest.Touch(file.Name);

    public static IEnumerable<BuildSystemDependency> CopyFiles(this DirectoryInfo src, DirectoryInfo dest) =>
        src.GetFiles()
            .Select(f => f.CopyTo(dest));

    public static BuildSystemDependency CopyTo(this FileInfo from, DirectoryInfo to) =>
        from.CopyAs(from.TouchCopy(to));

    public static BuildSystemDependency CopyAs(this FileInfo from, FileInfo to) => 
        new ()
        {
            Name = $"{to.Directory?.Name}/{from.Name}",
            From = from,
            To = to
        };
}
