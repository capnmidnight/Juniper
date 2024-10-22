namespace BitMiracle.LibJpeg.Classic.Internal;

internal class ArithEntropyDecoder : JpegEntropyDecoder
{
    public ArithEntropyDecoder(JpegDecompressStruct cinfo)
    {
        cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
    }

    public override void StartPass()
    {
        throw new NotImplementedException();
    }
}
