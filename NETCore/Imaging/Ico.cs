// Ignore Spelling: Ico

using System.Runtime.InteropServices;

namespace Juniper.Imaging.Ico;

public static partial class Ico
{
    private static T ReadStruct<T>(Stream stream) where T: struct
    {
        var size = Marshal.SizeOf<T>();
        var data = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        stream.Read(data, 0, size);
        Marshal.Copy(data, 0, ptr, size);
        var strct = Marshal.PtrToStructure<T>(ptr);
        return strct;
    }

    private static int WriteStruct<T>(Stream stream, T value) where T: struct
    {
        var size = Marshal.SizeOf<T>();
        var data = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(value, ptr, false);
        Marshal.Copy(ptr, data, 0, size);
        stream.Write(data, 0, size);
        return size;
    }

    private static IcoImageFileRaw LoadRaw(FileInfo file)
    {
        using var stream = file.OpenRead();
        var header = ReadStruct<IcoHeader>(stream);
        var imageDirectory = new IcoImageDirectoryEntry[header.NumImages];
        var imageData = new byte[header.NumImages][];

        for(var i = 0; i < header.NumImages; ++i)
        {
            imageDirectory[i] = ReadStruct<IcoImageDirectoryEntry>(stream);            
        }

        for(var i = 0; i < header.NumImages; ++i)
        {
            imageData[i] = new byte[imageDirectory[i].ImageDataLength];
            stream.Read(imageData[i], 0, imageData[i].Length);
        }

        return new IcoImageFileRaw
        {
            Header = header,
            ImageDirectory = imageDirectory,
            ImageData = imageData
        };
    }

    public static ImageData[] Load(FileInfo file)
    {
        var data = LoadRaw(file);
        foreach(var imgData in data.ImageData)
        {
            if (!ImageInfo.IsPNG(imgData))
            {
                throw new Exception("This library only knows how to handle PNG-based ICOs");
            }
        }

        var images = new ImageData[data.ImageData.Length];
        var factory = new PngFactory();
        var codec = new PngCodec();
        for(var i = 0; i < images.Length; ++i)
        {
            using var stream = new MemoryStream(data.ImageData[i]);
            images[i] = codec.Translate(factory.Dequeue(stream));
        }

        return images;
    }

    public static void Save(FileInfo file, ImageData[] images)
    {
        using var stream = file.OpenWrite();
        var header = new IcoHeader(IcoType.Ico, images.Length);
        var offset = WriteStruct(stream, header);
        offset += images.Length * Marshal.SizeOf<IcoImageDirectoryEntry>();

        var data = new byte[images.Length][];
        var factory = new PngFactory();
        var codec = new PngCodec();
        for (var i = 0; i < images.Length; ++i)
        {
            var image = images[i];
            using var mem = new MemoryStream();
            factory.Serialize(mem, codec.Translate(image));
            data[i] = mem.ToArray();
            offset = WriteInfo(stream, offset, data[i], image.Info);
        }

        foreach (var block in data)
        {
            stream.Write(block);
        }
    }

    public static void Concatenate(FileInfo file, params FileInfo[] images)
    {
        using var stream = file.OpenWrite();
        var header = new IcoHeader(IcoType.Ico, images.Length);
        var offset = WriteStruct(stream, header);
        offset += images.Length * Marshal.SizeOf<IcoImageDirectoryEntry>();

        var data = new byte[images.Length][];
        var factory = new PngFactory();
        var codec = new PngCodec();
        for (var i = 0; i < images.Length; ++i)
        {
            var image = images[i];
            data[i] = File.ReadAllBytes(image.FullName);
            var info = ImageInfo.ReadPNG(data[i]);
            offset = WriteInfo(stream, offset, data[i], info);
        }

        foreach (var block in data)
        {
            stream.Write(block);
        }

    }

    private static int WriteInfo(FileStream stream, int offset, byte[] data, ImageInfo info)
    {
        var entry = new IcoImageDirectoryEntry
        {
            Width = info.Dimensions.Width,
            Height = info.Dimensions.Height,
            ColorPlanes = (ushort)info.Components,
            ImageDataLength = (uint)data.Length,
            ImageDataOffset = (uint)offset
        };
        offset += data.Length;
        WriteStruct(stream, entry);
        return offset;
    }
}