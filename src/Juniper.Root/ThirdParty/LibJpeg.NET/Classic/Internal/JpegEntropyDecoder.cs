namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Entropy decoding
    /// </summary>
    internal abstract class JpegEntropyDecoder
    {
        public delegate bool DecodeMcuDelegate(JBlock[] MCU_data);
        public delegate void FinishPassDelegate();

        public DecodeMcuDelegate decodeMcu;
        public FinishPassDelegate finishPass;

        public abstract void StartPass();
    }
}
