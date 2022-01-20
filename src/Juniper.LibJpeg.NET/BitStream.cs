using System;
using System.IO;

namespace BitMiracle.LibJpeg
{
    internal class BitStream : IDisposable
    {
        private bool m_alreadyDisposed;

        private const int bitsInByte = 8;
        private Stream m_stream;
        private int m_positionInByte;

        private int m_size;

        public BitStream()
        {
            m_stream = new MemoryStream();
        }

        public BitStream(byte[] buffer)
        {
            if (buffer is null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            m_stream = new MemoryStream(buffer);
            m_size = BitsAllocated();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!m_alreadyDisposed)
            {
                if (disposing)
                {
                    m_stream?.Dispose();
                }

                m_stream = null;
                m_alreadyDisposed = true;
            }
        }

        public int Size()
        {
            return m_size;
        }

        public Stream UnderlyingStream => m_stream;

        public virtual int Read(int bitsCount)
        {
            if (Tell() + bitsCount > BitsAllocated())
            {
                throw new ArgumentOutOfRangeException(nameof(bitsCount));
            }

            //Codes are packed into a continuous bit stream, high-order bit first. 
            //This stream is then divided into 8-bit bytes, high-order bit first. 
            //Thus, codes can straddle byte boundaries arbitrarily. After the EOD marker (code value 257), 
            //any leftover bits in the final byte are set to 0.
            if (bitsCount < 0 || bitsCount > 32)
            {
                throw new ArgumentOutOfRangeException(nameof(bitsCount));
            }

            if (bitsCount == 0)
            {
                return 0;
            }

            var bitsRead = 0;
            var result = 0;
            var bt = new byte[1];
            while (bitsRead == 0 || (bitsRead - m_positionInByte < bitsCount))
            {
                m_stream.Read(bt, 0, 1);

                result <<= bitsInByte;
                result += bt[0];

                bitsRead += 8;
            }

            m_positionInByte = (m_positionInByte + bitsCount) % 8;
            if (m_positionInByte != 0)
            {
                result >>= (bitsInByte - m_positionInByte);

                m_stream.Seek(-1, SeekOrigin.Current);
            }

            if (bitsCount < 32)
            {
                var mask = ((1 << bitsCount) - 1);
                result &= mask;
            }

            return result;
        }

        public int Write(int bitStorage, int bitCount)
        {
            if (bitCount == 0)
            {
                return 0;
            }

            const int maxBitsInStorage = sizeof(int) * bitsInByte;
            if (bitCount > maxBitsInStorage)
            {
                throw new ArgumentOutOfRangeException(nameof(bitCount));
            }

            for (var i = 0; i < bitCount; ++i)
            {
                var bit = (byte)((bitStorage << (maxBitsInStorage - (bitCount - i))) >> (maxBitsInStorage - 1));
                if (!WriteBit(bit))
                {
                    return i;
                }
            }

            return bitCount;
        }

        public void Seek(int pos, SeekOrigin mode)
        {
            switch (mode)
            {
                case SeekOrigin.Begin:
                SeekSet(pos);
                break;

                case SeekOrigin.Current:
                SeekCurrent(pos);
                break;

                case SeekOrigin.End:
                SeekSet(Size() + pos);
                break;
            }
        }

        public int Tell()
        {
            return ((int)m_stream.Position * bitsInByte) + m_positionInByte;
        }

        private int BitsAllocated()
        {
            return (int)m_stream.Length * bitsInByte;
        }

        private bool WriteBit(byte bit)
        {
            if (m_stream.Position == m_stream.Length)
            {
                byte[] bt = { (byte)(bit << (bitsInByte - 1)) };
                m_stream.Write(bt, 0, 1);
                m_stream.Seek(-1, SeekOrigin.Current);
            }
            else
            {
                byte[] bt = { 0 };
                m_stream.Read(bt, 0, 1);
                m_stream.Seek(-1, SeekOrigin.Current);

                var shift = (bitsInByte - m_positionInByte - 1) % bitsInByte;
                var maskByte = (byte)(bit << shift);

                bt[0] |= maskByte;
                m_stream.Write(bt, 0, 1);
                m_stream.Seek(-1, SeekOrigin.Current);
            }

            Seek(1, SeekOrigin.Current);

            var currentPosition = Tell();
            if (currentPosition > m_size)
            {
                m_size = currentPosition;
            }

            return true;
        }

        private void SeekSet(int pos)
        {
            if (pos < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pos));
            }

            var byteDisplacement = pos / bitsInByte;
            m_stream.Seek(byteDisplacement, SeekOrigin.Begin);

            m_positionInByte = pos - (byteDisplacement * bitsInByte);
        }

        private void SeekCurrent(int pos)
        {
            var result = Tell() + pos;
            if (result < 0 || result > BitsAllocated())
            {
                throw new ArgumentOutOfRangeException(nameof(pos));
            }

            SeekSet(result);
        }
    }
}
