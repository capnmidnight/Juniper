using NAudio.Wave;

namespace NLayer.NAudioSupport
{
    public sealed class Mp3FrameDecompressor : IMp3FrameDecompressor
    {
        private readonly MpegFrameDecoder decoder;
        private readonly Mp3FrameWrapper frame;

        public Mp3FrameDecompressor(WaveFormat waveFormat)
        {
            // we assume waveFormat was calculated from the first frame already
            OutputFormat = WaveFormat.CreateIeeeFloatWaveFormat(waveFormat.SampleRate, waveFormat.Channels);

            decoder = new MpegFrameDecoder();
            frame = new Mp3FrameWrapper();
        }

        public int DecompressFrame(Mp3Frame frame, byte[] dest, int destOffset)
        {
            this.frame.WrappedFrame = frame;
            return decoder.DecodeFrame(this.frame, dest, destOffset);
        }

        public WaveFormat OutputFormat { get; }

        public void SetEQ(float[] eq)
        {
            decoder.SetEQ(eq);
        }

        public StereoMode StereoMode
        {
            get { return decoder.StereoMode; }
            set { decoder.StereoMode = value; }
        }

        public void Reset()
        {
            decoder.Reset();
        }

        public void Dispose()
        {
            // no-op, since we don't have anything to do here...
        }
    }
}
