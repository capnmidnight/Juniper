namespace Hjg.Pngcs.Zlib;

public class CRC32
{ // based on http://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net
    private const uint defaultPolynomial = 0xedb88320;
    private const uint defaultSeed = 0xffffffff;
    private static uint[] defaultTable;

    private uint hash;
    private readonly uint seed;
    private readonly uint[] table;

    public CRC32()
        : this(defaultPolynomial, defaultSeed)
    {
    }

    public CRC32(uint polynomial, uint seed)
    {
        table = InitializeTable(polynomial);
        this.seed = seed;
        hash = seed;
    }

    public void Update(byte[] buffer)
    {
        if (buffer is null)
        {
            throw new System.ArgumentNullException(nameof(buffer));
        }

        Update(buffer, 0, buffer.Length);
    }

    public void Update(byte[] buffer, int start, int length)
    {
        if (buffer is null)
        {
            throw new System.ArgumentNullException(nameof(buffer));
        }

        for (int i = 0, j = start; i < length; i++, j++)
        {
            unchecked
            {
                hash = (hash >> 8) ^ table[buffer[j] ^ (hash & 0xff)];
            }
        }
    }

    public uint GetValue()
    {
        return ~hash;
    }

    public void Reset()
    {
        hash = seed;
    }

    private static uint[] InitializeTable(uint polynomial)
    {
        if (polynomial == defaultPolynomial && defaultTable is object)
        {
            return defaultTable;
        }

        var createTable = new uint[256];
        for (var i = 0; i < 256; i++)
        {
            var entry = (uint)i;
            for (var j = 0; j < 8; j++)
            {
                if ((entry & 1) == 1)
                {
                    entry = (entry >> 1) ^ polynomial;
                }
                else
                {
                    entry >>= 1;
                }
            }

            createTable[i] = entry;
        }

        if (polynomial == defaultPolynomial)
        {
            defaultTable = createTable;
        }

        return createTable;
    }
}