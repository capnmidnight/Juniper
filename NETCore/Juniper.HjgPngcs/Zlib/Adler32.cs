namespace Hjg.Pngcs.Zlib
{
    public class Adler32
    {
        private uint a = 1;
        private uint b;
        private const int _base = 65521; /* largest prime smaller than 65536 */
        private const int _nmax = 5550;
        private int pend; // how many bytes have I read witouth computing modulus

        public void Update(byte data)
        {
            if (pend >= _nmax)
            {
                UpdateModulus();
            }

            a += data;
            b += a;
            pend++;
        }

        public void Update(byte[] data)
        {
            if (data is null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }

            Update(data, 0, data.Length);
        }

        public void Update(byte[] data, int offset, int length)
        {
            if (data is null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }

            var nextJToComputeModulus = _nmax - pend;
            for (var j = 0; j < length; j++)
            {
                if (j == nextJToComputeModulus)
                {
                    UpdateModulus();
                    nextJToComputeModulus = j + _nmax;
                }

                unchecked
                {
                    a += data[j + offset];
                }

                b += a;
                pend++;
            }
        }

        public void Reset()
        {
            a = 1;
            b = 0;
            pend = 0;
        }

        private void UpdateModulus()
        {
            a %= _base;
            b %= _base;
            pend = 0;
        }

        public uint GetValue()
        {
            if (pend > 0)
            {
                UpdateModulus();
            }

            return (b << 16) | a;
        }
    }
}