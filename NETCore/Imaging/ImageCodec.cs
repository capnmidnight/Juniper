using Juniper.IO;
using Juniper.Progress;

namespace Juniper.Imaging;

public static class ImageTranscoder
{
    public static IImageCodec<FromImageT, ToImageT> Create<FromImageT, FromImageFactoryT, FromImageCodecT, IntermediateImageT, ToImageCodecT, ToImageFactoryT, ToImageT>(
        FromImageFactoryT fromFactory,
        FromImageCodecT fromCodec,
        ToImageCodecT toCodec,
        ToImageFactoryT toFactory
        )
        where FromImageFactoryT : IImageFactory<FromImageT>
        where FromImageCodecT : IImageCodec<FromImageT, IntermediateImageT>
        where ToImageFactoryT : IImageFactory<ToImageT>
        where ToImageCodecT : IImageCodec<IntermediateImageT, ToImageT>
    {
        return new ImageTranscoder<FromImageT, FromImageFactoryT, FromImageCodecT, IntermediateImageT, ToImageCodecT, ToImageFactoryT, ToImageT>(fromFactory, fromCodec, toCodec, toFactory);
    }
}

public class ImageTranscoder<FromImageT, FromImageFactoryT, FromImageCodecT, IntermediateImageT, ToImageCodecT, ToImageFactoryT, ToImageT>
    : IImageCodec<FromImageT, ToImageT>,
      IDeserializer<FromImageT, MediaType.Image>,
      ISerializer<ToImageT, MediaType.Image>
        where FromImageFactoryT : IImageFactory<FromImageT>
        where FromImageCodecT : IImageCodec<FromImageT, IntermediateImageT>
        where ToImageFactoryT : IImageFactory<ToImageT>
        where ToImageCodecT : IImageCodec<IntermediateImageT, ToImageT>
{
    private readonly FromImageFactoryT fromFactory;
    private readonly FromImageCodecT fromCodec;
    private readonly ToImageCodecT toCodec;
    private readonly ToImageFactoryT toFactory;

    public ImageTranscoder(FromImageFactoryT fromFactory, FromImageCodecT fromCodec, ToImageCodecT toCodec, ToImageFactoryT toFactory)
    {
        this.fromFactory = fromFactory;
        this.fromCodec = fromCodec;
        this.toCodec = toCodec;
        this.toFactory = toFactory;
    }

    public MediaType.Image? InputContentType => fromFactory.InputContentType;

    public MediaType.Image OutputContentType => toFactory.OutputContentType;

    public FromImageT? Deserialize(Stream stream)
    {
        return fromFactory.Deserialize(stream);
    }

    public long Serialize(Stream stream, ToImageT value)
    {
        return toFactory.Serialize(stream, value);
    }

    public ToImageT Translate(FromImageT value, IProgress? prog = null)
    {
        var progs = prog.Split(2);
        return toCodec.Translate(fromCodec.Translate(value, progs[0]), progs[1]);
    }

    public FromImageT Translate(ToImageT image, IProgress? prog = null)
    {
        var progs = prog.Split(2);
        return fromCodec.Translate(toCodec.Translate(image, progs[0]), progs[1]);
    }

    public void Translate(Stream inFile, Stream outFile, IProgress? prog = null)
    {
        var progs = prog.Split(3);
        var from = fromFactory.Deserialize(inFile, progs[0]);
        if (from is not null)
        {
            var to = Translate(from, progs[1]);
            toFactory.Serialize(outFile, to);
            progs[2].Report(1);
        }
    }

    public void Translate(FileInfo inFile, FileInfo outFile, IProgress? prog = null)
    {
        var progs = prog.Split(3);
        var from = fromFactory.Deserialize(inFile, progs[0]);
        if (from is not null)
        {
            var to = Translate(from, progs[1]);
            toFactory.Serialize(outFile, to);
            progs[2].Report(1);
        }
    }
}


public static class ImageCodec
{
    public static IImageFactory<ToImageT> Create<ImageFactoryT, FromImageT, ImageCodecT, ToImageT>(ImageFactoryT factory, ImageCodecT codec)
        where ImageFactoryT : IImageFactory<FromImageT>
        where ImageCodecT : IImageCodec<FromImageT, ToImageT>
    {
        return new ImageCodec<FromImageT, ToImageT>(factory, codec);
    }
}

public class ImageCodec<FromImageT, ToImageT>
    : IImageFactory<ToImageT>
{
    private readonly IImageFactory<FromImageT> factory;
    private readonly IImageCodec<FromImageT, ToImageT> codec;

    public ImageCodec(IImageFactory<FromImageT> factory, IImageCodec<FromImageT, ToImageT> codec)
    {
        this.factory = factory;
        this.codec = codec;
    }

    public MediaType.Image? InputContentType => factory.InputContentType;

    public MediaType.Image OutputContentType => MediaType.Image_Raw;

    public ToImageT? Deserialize(Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        var img = factory.Deserialize(stream);
        if(img is null)
        {
            return default;
        }

        return codec.Translate(img);
    }

    public long Serialize(Stream stream, ToImageT value)
    {
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var img = codec.Translate(value);
        return factory.Serialize(stream, img);
    }
}
