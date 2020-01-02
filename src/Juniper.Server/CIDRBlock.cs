using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Juniper.HTTP.Server
{
    public sealed class CIDRBlock :
        ICollection<IPAddress>
    {
        private static readonly Dictionary<AddressFamily, int> BitLengths = new Dictionary<AddressFamily, int>
        {
            [AddressFamily.InterNetwork] = Units.Bits.PER_BYTE * 4,
            [AddressFamily.InterNetworkV6] = Units.Bits.PER_BYTE * 16
        };

        public static CIDRBlock[] Load(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanRead)
            {
                throw new AccessViolationException($"Cannot read from {nameof(stream)}.");
            }

            var blocks = new List<CIDRBlock>();
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (TryParse(line, out var block))
                    {
                        blocks.Add(block);
                    }
                }
            }

            return blocks.ToArray();
        }

        public static CIDRBlock[] Load(FileInfo file)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                return Array.Empty<CIDRBlock>();
            }

            using var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
            return Load(stream);
        }

        public static CIDRBlock[] Load(string fileName)
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            return Load(new FileInfo(fileName));
        }

        private static void Save(IEnumerable<CIDRBlock> blocks, Stream stream)
        {
            if (blocks is null)
            {
                throw new ArgumentNullException(nameof(blocks));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (!stream.CanWrite)
            {
                throw new AccessViolationException($"Cannot write to {nameof(stream)}.");
            }

            using var writer = new StreamWriter(stream);
            foreach (var block in blocks)
            {
                writer.WriteLine(block.ToString());
            }
        }

        public static void Save(IEnumerable<CIDRBlock> blocks, FileInfo file)
        {
            if (blocks is null)
            {
                throw new ArgumentNullException(nameof(blocks));
            }

            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var stream = file.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            Save(blocks, stream);
        }

        public static void Save(IEnumerable<CIDRBlock> blocks, string fileName)
        {
            if (blocks is null)
            {
                throw new ArgumentNullException(nameof(blocks));
            }

            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            Save(blocks, new FileInfo(fileName));
        }

        public static CIDRBlock Parse(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length == 0)
            {
                throw new FormatException("Empty string does not specify a valid CIDR block.");
            }

            if (!value.Contains('/'))
            {
                throw new FormatException($"'{value}' does not specify a valid CIDR block. It does not contain a '/' character.");
            }

            var parts = value.SplitX('/');
            if (parts.Length != 2)
            {
                throw new FormatException($"'{value}' does not specify a valid CIDR block. It contains more than one '/' character.");
            }

            if (!IPAddress.TryParse(parts[0], out var start))
            {
                throw new FormatException($"'{value}' does not specify a valid CIDR block. The first half is not a valid IP address.");
            }

            if (!int.TryParse(parts[1], out var bitmaskLength))
            {
                throw new FormatException($"'{value}' does not specify a valid CIDR block. The second half is not a valid int value.");
            }

            return new CIDRBlock(start, bitmaskLength);
        }

        public static bool TryParse(string value, out CIDRBlock block)
        {
            block = null;

            if (value is object)
            {
                var parts = value.SplitX('/');

                if (parts.Length == 2
                    && IPAddress.TryParse(parts[0], out var start)
                    && int.TryParse(parts[1], out var bitmaskLength)
                    && BitLengths.ContainsKey(start.AddressFamily)
                    && bitmaskLength <= BitLengths[start.AddressFamily])
                {
                    block = new CIDRBlock(start, bitmaskLength);
                }
            }

            return block is object;
        }

        private static void ValidateStart(IPAddress start)
        {
            if (start is null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (!BitLengths.ContainsKey(start.AddressFamily))
            {
                throw new NotSupportedException($"{start.AddressFamily.ToString()} is not a supported address type.");
            }
        }

        private static void ValidateEnd(IPAddress start, IPAddress end)
        {
            if (end is null)
            {
                throw new ArgumentNullException(nameof(end));
            }

            if (start.AddressFamily != end.AddressFamily)
            {
                throw new InvalidOperationException($"Both {nameof(start)} and {nameof(end)} addresses must be of the same family: {nameof(start)} is {start.AddressFamily.ToString()} and {nameof(end)} is {end.AddressFamily.ToString()}");
            }
        }

        private static void ValidateBitmaskLength(IPAddress start, int bitmaskLength)
        {
            if (bitmaskLength > BitLengths[start.AddressFamily])
            {
                throw new ArgumentOutOfRangeException(nameof(bitmaskLength), $"{bitmaskLength.ToString(CultureInfo.CurrentCulture)} is too large for {start.AddressFamily.ToString()} addresses. Expected: {BitLengths[start.AddressFamily].ToString(CultureInfo.CurrentCulture)}");
            }
        }

        private static void ValidateStartEndOrder(IPAddress start, IPAddress end)
        {
            var startBytes = start.GetAddressBytes();
            var endBytes = end.GetAddressBytes();
            for (var i = 0; i < startBytes.Length; ++i)
            {
                if (endBytes[i] < startBytes[i])
                {
                    throw new InvalidOperationException($"{nameof(end)} address must be greater than or equal to {nameof(start)} address.");
                }
            }
        }

        private static int CalculateBitmaskLength(IPAddress start, IPAddress end)
        {
            ValidateStart(start);
            ValidateEnd(start, end);
            ValidateStartEndOrder(start, end);

            var startBytes = start.GetAddressBytes();
            var endBytes = end.GetAddressBytes();
            var bitmaskLength = 0;

            for (var i = 0; i < startBytes.Length; ++i)
            {
                if (startBytes[i] == endBytes[i])
                {
                    bitmaskLength += Units.Bits.PER_BYTE;
                }
                else
                {
                    byte mask = 0x80;
                    for (var j = 0; j < Units.Bits.PER_BYTE; ++j)
                    {
                        if (((startBytes[i] ^ endBytes[i]) & mask) == 0)
                        {
                            ++bitmaskLength;
                        }
                        else
                        {
                            break;
                        }

                        mask >>= 1;
                    }

                    break;
                }
            }

            return bitmaskLength;
        }

        private static IPAddress CalculateEnd(IPAddress start, int bitmaskLength)
        {
            ValidateStart(start);
            ValidateBitmaskLength(start, bitmaskLength);

            var startBytes = start.GetAddressBytes();
            var endBytes = startBytes.ToArray();

            for (var i = 0; i < startBytes.Length; ++i)
            {
                endBytes[i] = startBytes[i];

                if (bitmaskLength >= Units.Bits.PER_BYTE)
                {
                    bitmaskLength -= Units.Bits.PER_BYTE;
                }
                else
                {
                    endBytes[i] |= (byte)(byte.MaxValue >> bitmaskLength);

                    for (var j = i + 1; j < startBytes.Length; ++j)
                    {
                        endBytes[j] = byte.MaxValue;
                    }

                    break;
                }
            }

            return new IPAddress(endBytes);
        }

        private readonly byte[] startBytes;
        private readonly byte[] endBytes;

        public bool IsReadOnly
        {
            get { return true; }
        }

        public int Count { get; }

        public IPAddress Start { get; }

        public IPAddress End { get; }

        public IPAddress SubnetMask { get; }

        public int BitmaskLength { get; }

        private CIDRBlock(IPAddress start, IPAddress end, int bitmaskLength)
        {
            Start = start;
            startBytes = start.GetAddressBytes();

            End = end;
            endBytes = end.GetAddressBytes();

            BitmaskLength = bitmaskLength;
            Count = (int)Math.Pow(2, BitLengths[Start.AddressFamily] - BitmaskLength);

            var subnetBytes = new byte[startBytes.Length];
            for (var i = 0; i < subnetBytes.Length; ++i)
            {
                if (bitmaskLength >= Units.Bits.PER_BYTE)
                {
                    subnetBytes[i] = byte.MaxValue;
                    bitmaskLength -= Units.Bits.PER_BYTE;
                }
                else
                {
                    subnetBytes[i] = (byte)(byte.MaxValue << Units.Bits.PER_BYTE - bitmaskLength);
                    break;
                }
            }

            SubnetMask = new IPAddress(subnetBytes);
        }

        public CIDRBlock(IPAddress start, IPAddress end)
            : this(start, end, CalculateBitmaskLength(start, end))
        { }

        public CIDRBlock(IPAddress start)
            : this(start, start)
        { }

        public CIDRBlock(IPAddress start, int bitmaskLength)
            : this(start, CalculateEnd(start, bitmaskLength), bitmaskLength)
        { }

        /// <summary>
        /// Returns a string that represents the current CIDR Block. This is
        /// represented as the stringified <see cref="Start"/> address, separated
        /// from the <see cref="BitmaskLength"/> with a '/' character.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return Start.ToString()
                + "/"
                + BitmaskLength.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Determines whether the <see cref="ICollection{T}" /> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="ICollection{T}" />.</param>
        /// <returns>
        ///     <see langword="true" /> if <paramref name="item" /> is found in the <see cref="ICollection{T}" />; otherwise, <see langword="false" />.</returns>
        /// <exception cref="ArgumentNullException">
        ///         <paramref name="item" /> is <see langword="null" />.</exception>
        public bool Contains(IPAddress item)
        {
            if (item is null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (item.AddressFamily != Start.AddressFamily)
            {
                return false;
            }

            var itemBytes = item.GetAddressBytes();
            for (var i = 0; i < itemBytes.Length; ++i)
            {
                if (startBytes[i] > itemBytes[i]
                    || itemBytes[i] > endBytes[i])
                {
                    return false;
                }
            }

            return true;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IPAddress> GetEnumerator()
        {
            var itemBytes = startBytes.ToArray();

            for (var i = 0; i < Count; ++i)
            {
                yield return new IPAddress(itemBytes);

                for (var j = itemBytes.Length - 1; j >= 0; --j)
                {
                    ++itemBytes[j];
                    if (itemBytes[j] != 0)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Copies the elements of the <see cref="ICollection{T}" /> to an <see cref="Array" />, starting at a particular <see cref="Array" /> index.
        /// </summary>
        /// <param name="array">The one-dimensional <see cref="Array" /> that is the destination of the elements copied from <see cref="ICollection{T}" />. The <see cref="Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="ArgumentNullException">
        ///         <paramref name="array" /> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///         <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="ArgumentException">The number of elements in the source <see cref="ICollection{T}" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(IPAddress[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("The number of elements in this collection is greater than the available space in the destination array.");
            }

            foreach (var address in this)
            {
                array[arrayIndex++] = address;
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="NotSupportedException">The <see cref="ICollection{T}" /> is read-only.</exception>
        public bool Remove(IPAddress item)
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">The <see cref="ICollection{T}" /> is read-only.</exception>
        public void Add(IPAddress item)
        {
            throw new NotSupportedException("This collection is read-only.");
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">The <see cref="ICollection{T}" /> is read-only.</exception>
        public void Clear()
        {
            throw new NotSupportedException("This collection is read-only.");
        }
    }
}
