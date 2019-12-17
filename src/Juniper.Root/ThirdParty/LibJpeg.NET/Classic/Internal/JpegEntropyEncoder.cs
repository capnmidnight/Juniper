namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Entropy encoding
    /// </summary>
    internal abstract class JpegEntropyEncoder
    {
        public delegate bool EncodeMcuDelegate(JBlock[][] MCU_data);
        public delegate void FinishPassDelegate();

        public EncodeMcuDelegate encodeMcu;
        public FinishPassDelegate finishPass;

        public abstract void StartPass(bool gather_statistics);
    }
}
