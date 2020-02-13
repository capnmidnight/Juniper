namespace BitMiracle.LibJpeg.Classic.Internal
{
    /// <summary>
    /// Encapsulates buffer of image samples for one color component
    /// When provided with funny indices (see jpeg_d_main_controller for 
    /// explanation of what it is) uses them for non-linear row access.
    /// </summary>
    internal class ComponentBuffer
    {
        private byte[][] buffer;

        // array of funny indices
        private int[] funnyIndices;

        // index of "first funny index" (used because some code uses negative 
        // indices when retrieve rows)
        // see for example my_upsampler.h2v2_fancy_upsample
        private int funnyOffset;

        public ComponentBuffer()
        {
        }

        public ComponentBuffer(byte[][] buf, int[] funnyIndices, int funnyOffset)
        {
            SetBuffer(buf, funnyIndices, funnyOffset);
        }

        public byte[] this[int i]
        {
            get
            {
                if (funnyIndices is null)
                {
                    return buffer[i];
                }

                return buffer[funnyIndices[i + funnyOffset]];
            }
        }

        public void SetBuffer(byte[][] buf, int[] funnyIndices, int funnyOffset)
        {
            buffer = buf;
            this.funnyIndices = funnyIndices;
            this.funnyOffset = funnyOffset;
        }
    }
}
