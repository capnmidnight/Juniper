namespace Juniper.TSBuild
{

    public struct BuildSystemDependency
    {
        public string Name { get; set; }
        public FileInfo From { get; set; }
        public FileInfo To { get; set; }
        
        public void Deconstruct(out string Name, out FileInfo From, out FileInfo To)
        {
            Name = this.Name;
            From = this.From;
            To = this.To;
        }
    }

    public static class FileInfoExtensions
    {
        public static FileInfo TouchCopy(this FileInfo file, DirectoryInfo dest)
        {
            return dest.Touch(file.Name);
        }

        public static IEnumerable<BuildSystemDependency> CopyFiles(this DirectoryInfo src, DirectoryInfo dest)
        {
            return src.GetFiles()
                .Select(f => f.MakeDependency(dest));
        }

        public static BuildSystemDependency MakeDependency(this FileInfo from, DirectoryInfo to)
        {
            return new BuildSystemDependency
            {
                Name = $"{to.Name}/${from.Name}",
                From = from,
                To = from.TouchCopy(to)
            };
        }
    }
}
