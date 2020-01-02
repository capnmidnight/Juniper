using System;

namespace NLayer.NAudioSupport
{
    internal class Mp3FrameWrapper : IMpegFrame
    {
        private NAudio.Wave.Mp3Frame _frame;

        internal NAudio.Wave.Mp3Frame WrappedFrame
        {
            set
            {
                _frame = value;
                Reset();
            }
        }

        public int SampleRate
        {
            get { return _frame.SampleRate; }
        }

        public int SampleRateIndex
        {
            // we have to manually parse this out
            get
            {
                // sri is in bits 10 & 11 of the sync DWORD...  pull them out
                return (_frame.RawData[2] >> 2) & 3;
            }
        }

        public int FrameLength
        {
            get { return _frame.FrameLength; }
        }

        public int BitRate
        {
            get { return _frame.BitRate; }
        }

        public MpegVersion Version
        {
            get
            {
                return _frame.MpegVersion switch
                {
                    NAudio.Wave.MpegVersion.Version1 => MpegVersion.Version1,
                    NAudio.Wave.MpegVersion.Version2 => MpegVersion.Version2,
                    NAudio.Wave.MpegVersion.Version25 => MpegVersion.Version25,
                    _ => MpegVersion.Unknown,
                };
            }
        }

        public MpegLayer Layer
        {
            get
            {
                return _frame.MpegLayer switch
                {
                    NAudio.Wave.MpegLayer.Layer1 => MpegLayer.LayerI,
                    NAudio.Wave.MpegLayer.Layer2 => MpegLayer.LayerII,
                    NAudio.Wave.MpegLayer.Layer3 => MpegLayer.LayerIII,
                    _ => MpegLayer.Unknown,
                };
            }
        }

        public MpegChannelMode ChannelMode
        {
            get
            {
                return _frame.ChannelMode switch
                {
                    NAudio.Wave.ChannelMode.Stereo => MpegChannelMode.Stereo,
                    NAudio.Wave.ChannelMode.JointStereo => MpegChannelMode.JointStereo,
                    NAudio.Wave.ChannelMode.DualChannel => MpegChannelMode.DualChannel,
                    NAudio.Wave.ChannelMode.Mono => MpegChannelMode.Mono,
                    _ => (MpegChannelMode)(-1),
                };
            }
        }

        public int ChannelModeExtension
        {
            get { return _frame.ChannelExtension; }
        }

        public int SampleCount
        {
            get { return _frame.SampleCount; }
        }

        public int BitRateIndex
        {
            get { return _frame.BitRateIndex; }
        }

        public bool IsCopyrighted
        {
            get { return _frame.Copyright; }
        }

        public bool HasCrc
        {
            get { return _frame.CrcPresent; }
        }

        // we assume everything is good here since NAudio should've already caught any errors
        public bool IsCorrupted
        {
            get { return false; }
        }

        private int _readOffset;
        private int _bitsRead;
        private ulong _bitBucket;

        public void Reset()
        {
            if (_frame is object)
            {
                _readOffset = (_frame.CrcPresent ? 2 : 0) + 4;
            }

            _bitsRead = 0;
        }

        public int ReadBits(int bitCount)
        {
            if (bitCount < 1 || bitCount > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(bitCount));
            }

            while (_bitsRead < bitCount)
            {
                if (_readOffset == _frame.FrameLength)
                {
                    throw new System.IO.EndOfStreamException();
                }

                var b = _frame.RawData[_readOffset++];
                _bitBucket <<= Juniper.Units.Bits.PER_BYTE;
                _bitBucket |= (byte)(b & 0xFF);
                _bitsRead += Juniper.Units.Bits.PER_BYTE;
            }

            var temp = (int)((_bitBucket >> (_bitsRead - bitCount)) & ((1UL << bitCount) - 1));
            _bitsRead -= bitCount;
            return temp;
        }
    }
}
