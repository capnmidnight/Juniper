using System;

namespace BitMiracle.LibJpeg.Classic.Internal
{
    internal class ArithEntropyDecoder : JpegEntropyDecoder
    {
        public ArithEntropyDecoder(jpeg_decompress_struct cinfo)
        {
            cinfo.ERREXIT(J_MESSAGE_CODE.JERR_NOTIMPL);
        }

        public override void StartPass()
        {
            throw new NotImplementedException();
        }
    }
}
