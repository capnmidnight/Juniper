namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Coefficient buffer control
    /// </summary>
    internal interface JpegCCoefController
    {
        void StartPass(JBufMode pass_mode);
        bool CompressData(byte[][][] input_buf);
    }
}
