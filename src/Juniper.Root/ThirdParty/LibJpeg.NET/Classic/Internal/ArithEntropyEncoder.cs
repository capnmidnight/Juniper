using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    internal class ArithEntropyEncoder : JpegEntropyEncoder
    {
        public ArithEntropyEncoder(jpeg_compress_struct cinfo)
        {
            cinfo.ERREXIT(J_MESSAGE_CODE.JERR_NOTIMPL);
        }

        public override void StartPass(bool gather_statistics)
        {
            throw new NotImplementedException();
        }
    }
}
