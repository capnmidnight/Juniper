using Juniper.IO;

using Xabe.FFmpeg;
using Xabe.FFmpeg.Downloader;

namespace Juniper.Sound;

public enum EZFFMPEGFormat
{
    Wav,
    MP3,
    WebMOpus,
    OggOpus,
    Raw
}

public static class EZFFMPEG
{
    private static readonly Dictionary<EZFFMPEGFormat, (MediaType, Format, AudioCodec)> parameters = new()
    {
        { EZFFMPEGFormat.WebMOpus, (MediaType.Audio_WebMOpus, Format.matroska, AudioCodec.libopus) },
        { EZFFMPEGFormat.OggOpus, (MediaType.Audio_OggOpus , Format.ogg, AudioCodec.libopus) },
        { EZFFMPEGFormat.MP3, (MediaType.Audio_Mpeg , Format.mp3, AudioCodec.mp3) },
        { EZFFMPEGFormat.Wav, (MediaType.Audio_Wav , Format.wav, AudioCodec.pcm_s16le) }
    };

    private static readonly Dictionary<EZFFMPEGFormat, (MediaType, string, string)> streamParameters = new()
    {
        { EZFFMPEGFormat.WebMOpus, (MediaType.Audio_WebMOpus, "matroska", "libopus") },
        { EZFFMPEGFormat.OggOpus, (MediaType.Audio_OggOpus , "ogg", "libopus") },
        { EZFFMPEGFormat.MP3, (MediaType.Audio_Mpeg , "mp3", "mp3") },
        { EZFFMPEGFormat.Wav, (MediaType.Audio_Wav , "wav", "pcm_s16le") },
        { EZFFMPEGFormat.Raw, (MediaType.Audio_Raw , "concat", "pcm_s16le") }
    };

    private static async Task InitFFMpeg()
    {
        if (FFmpeg.ExecutablesPath is null)
        {
            //Set directory where app should look for FFmpeg executables.
            var path = Processes.ShellCommand.FindCommandPath("ffmpeg");
            if (path is null)
            {
                path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
                var dir = new DirectoryInfo(path);
                if (dir.Exists)
                {
                    path = dir.GetFiles()
                        .Where(f => Path.GetFileNameWithoutExtension(f.Name).ToLower() == "ffmpeg")
                        .Select(f => f.FullName)
                        .FirstOrDefault();
                }
            }

            FFmpeg.SetExecutablesPath(Path.GetDirectoryName(path));
            if (!File.Exists(path))
            {
                //Get latest version of FFmpeg.
                await FFmpegDownloader.GetLatestVersion(FFmpegVersion.Official);
            }
        }
    }

    public static void ValidateInputMediaTypes(params MediaType[] mediaTypeIns)
    {
        if (mediaTypeIns is null)
        {
            throw new ArgumentNullException(nameof(mediaTypeIns));
        }

        var acceptableTypes = mediaTypeIns
            .Where(mediaTypeIn => mediaTypeIn is not null
                && (mediaTypeIn.Type == "audio"
                    || mediaTypeIn.Type == "video"));

        if (acceptableTypes.Empty())
        {
            throw new InvalidOperationException("No valid media types found");
        }
    }

    public static FFMpegProcessStream CreateFFMpegStream(EZFFMPEGFormat ezformat)
    {
        var (mediaTypeOut, format, codec) = streamParameters[ezformat];            
        return new FFMpegProcessStream(mediaTypeOut, format, codec);
    }

    public static async Task<TempFile> ConvertAsync(Stream streamIn, MediaType mediaTypeIn, EZFFMPEGFormat format)
    {
        ValidateInputMediaTypes(mediaTypeIn);

        using var fileIn = new TempFile(mediaTypeIn);
        await streamIn.CopyToAsync(fileIn);

        return await ConvertAsync(format, fileIn.FileInfo, mediaTypeIn);
    }

    public static async Task<TempFile> ConvertAsync(FileInfo file, EZFFMPEGFormat format)
    {
        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        if (!file.Exists)
        {
            throw new FileNotFoundException(file.FullName);
        }

        var types = file.GuessMediaType().ToArray();

        return await ConvertAsync(format, file, types);
    }

    private static async Task<TempFile> ConvertAsync(EZFFMPEGFormat format, FileInfo file, params MediaType[] types)
    {

        ValidateInputMediaTypes(types);
        return await ConvertAsync(file.FullName, format);
    }

    public static async Task<TempFile> ConvertAsync(string path, EZFFMPEGFormat format)
    {
        if (path is null)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        if (!parameters.ContainsKey(format))
        {
            throw new ArgumentOutOfRangeException(nameof(format), $"'{format}' is not a supported format.");
        }

        await InitFFMpeg();

        var mediaInfo = await FFmpeg.GetMediaInfo(path);
        var audioIn = mediaInfo.AudioStreams
            .OrderByDescending(v => v.Duration)
            .ThenByDescending(v => v.Bitrate)
            .FirstOrDefault();
        if (audioIn is null)
        {
            throw new InvalidOperationException("Couldn't find audio in file");
        }

        var (mediaTypeOut, ffmpegFormat, codec) = parameters[format];
        audioIn.SetCodec(codec);

        var fileOut = new TempFile(mediaTypeOut);
        var conversion = FFmpeg.Conversions.New()
            .AddParameter("-hide_banner")
            .AddStream(audioIn)
            .SetPreset(ConversionPreset.VerySlow)
            .SetOutput(fileOut.FilePath)
            .SetOutputFormat(ffmpegFormat);
        var conversionResult = await conversion.Start();
        if (conversionResult is null)
        {
            throw new InvalidOperationException("Media conversion failed");
        }

        return fileOut;
    }
}
