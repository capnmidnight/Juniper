namespace Juniper.IO;

public class FileDataSource : IDataSource
{
    public static readonly FileDataSource Instance = new();
    private FileDataSource()
    { }

    public Stream GetStream(string fileName)
    {
        if (fileName is null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (fileName.Length == 0)
        {
            throw new ArgumentException("path must not be empty string", nameof(fileName));
        }

        var file = new FileInfo(fileName);
        if (!file.Exists)
        {
            throw new FileNotFoundException("File not found!", file.FullName);
        }

        return file.OpenRead();
    }
}