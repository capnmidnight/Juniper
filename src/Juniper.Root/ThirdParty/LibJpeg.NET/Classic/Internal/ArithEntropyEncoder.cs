using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    internal class ArithEntropyEncoder : JpegEntropyEncoder
    {
        public ArithEntropyEncoder(JpegCompressStruct cinfo)
        {
            cinfo.ErrExit(JMessageCode.JERR_NOTIMPL);
        }

        public override void StartPass(bool gather_statistics)
        {
            throw new NotImplementedException();
        }
    }
}
