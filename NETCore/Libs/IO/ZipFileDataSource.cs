using System.IO.Compression;

namespace Juniper.IO;

public sealed class ZipFileDataSource : IDataSource, IDisposable
{
    private readonly ZipArchive zip;
    private readonly bool ownZip;

    public ZipFileDataSource(ZipArchive zip)
    {
        this.zip = zip ?? throw new ArgumentNullException(nameof(zip));
        ownZip = false;
    }

    public ZipFileDataSource(Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        zip = new ZipArchive(stream, ZipArchiveMode.Read, true);
        ownZip = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing
            && ownZip)
        {
            zip.Dispose();
        }
    }

    public Stream? GetStream(string fileName)
    {
        return zip.GetEntry(fileName)?.Open();
    }
}
