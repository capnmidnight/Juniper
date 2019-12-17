namespace BitMiracle.LibJpeg.Classic
{
    /// <summary>
    /// Data destination object for compression.
    /// </summary>
    public abstract class JpegDestinationManager
    {
        private byte[] buffer;
        private int position;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public abstract void InitDestination();

        /// <summary>
        /// Empties output buffer.
        /// </summary>
        /// <returns><c>true</c> if operation succeed; otherwise, <c>false</c></returns>
        public abstract bool EmptyOutputBuffer();

        /// <summary>
        /// Term_destinations this instance.
        /// </summary>
        public abstract void TermDestination();

        /// <summary>
        /// Emits a byte.
        /// </summary>
        /// <param name="val">The byte value.</param>
        /// <returns><c>true</c> if operation succeed; otherwise, <c>false</c></returns>
        public virtual bool EmitByte(int val)
        {
            buffer[position] = (byte)val;
            position++;

            if (--FreeInBuffer == 0)
            {
                if (!EmptyOutputBuffer())
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Initializes the internal buffer.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <param name="offset">The offset.</param>
        protected void InitInternalBuffer(byte[] buffer, int offset)
        {
            this.buffer = buffer;
            FreeInBuffer = buffer.Length - offset;
            position = offset;
        }

        /// <summary>
        /// Gets the number of free bytes in buffer.
        /// </summary>
        /// <value>The number of free bytes in buffer.</value>
        protected int FreeInBuffer { get; private set; }
    }
}
