using System;
using System.Runtime.InteropServices;

namespace IT.Hashing;

//https://github.com/MiloszKrajewski/K4os.Hash.Crc
public class CRC32
{
    private uint _seed;

    private readonly Table _table;

    public class Table
    {
        public static readonly Table Default = new Table();

        public uint[] Data { get; }

        public Table(uint polynomial)
        {
            Data = Build(polynomial);
        }

        public Table() : this(0xEDB88320) { }

        private static uint[] Build(uint polynomial)
        {
            var data = new uint[256];

            for (uint i = 0; i <= 255; i++)
            {
                var remainder = i;
                for (var j = 8; j > 0; --j)
                    remainder = (remainder >> 1) ^ ((remainder & 1) != 0 ? polynomial : 0);

                data[i] = remainder;
            }

            return data;
        }
    }

    public CRC32() : this(Table.Default) { }

    public CRC32(Table table)
    {
        _table = table ?? Table.Default;
    }

    private static unsafe uint DigestOf(
        uint[] table, byte* bytesP, int length, uint seed = 0)
    {
        if (length == 0)
            return seed;

        if (table == null || table.Length != 256)
            throw new ArgumentException("Invalid lookup table");

        seed = ~seed;

        fixed (uint* tableP = table)
        {
            while (length >= 8)
            {
                seed = (seed >> 8) ^ tableP[(bytesP[0] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[1] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[2] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[3] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[4] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[5] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[6] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[7] ^ seed) & 0xff];
                bytesP += 8;
                length -= 8;
            }

            if (length >= 4)
            {
                seed = (seed >> 8) ^ tableP[(bytesP[0] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[1] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[2] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[3] ^ seed) & 0xff];
                bytesP += 4;
                length -= 4;
            }

            if (length >= 2)
            {
                seed = (seed >> 8) ^ tableP[(bytesP[0] ^ seed) & 0xff];
                seed = (seed >> 8) ^ tableP[(bytesP[1] ^ seed) & 0xff];
                bytesP += 2;
                length -= 2;
            }

            if (length > 0)
            {
                seed = (seed >> 8) ^ tableP[(bytesP[0] ^ seed) & 0xff];
            }
        }

        return ~seed;
    }

    public static unsafe uint DigestOf(
        Table table, byte* bytes, int length, uint seed = 0) =>
        DigestOf(table.Data, bytes, length, seed);

    public static unsafe uint DigestOf(byte* bytes, int length, uint seed = 0) =>
        DigestOf(Table.Default, bytes, length, seed);

    public static unsafe uint DigestOf(
        Table table, ReadOnlySpan<byte> bytes, uint seed = 0)
    {
        fixed (byte* bytesP = &MemoryMarshal.GetReference(bytes))
            return DigestOf(table.Data, bytesP, bytes.Length, seed);
    }

    public static uint DigestOf(ReadOnlySpan<byte> bytes, uint seed = 0) =>
        DigestOf(Table.Default, bytes, seed);

    public static unsafe uint DigestOf(
        Table table, byte[] bytes, int offset, int length, uint seed = 0)
    {
        Validate(bytes, offset, length);

        fixed (byte* bytesP = bytes)
            return DigestOf(table.Data, bytesP + offset, length, seed);
    }

    public static uint DigestOf(byte[] bytes, int index, int length, uint seed = 0) =>
        DigestOf(Table.Default, bytes, index, length, seed);

    public void Reset() =>
        _seed = 0;

    public unsafe void Update(byte* bytesP, int length) =>
        _seed = DigestOf(_table, bytesP, length, _seed);

    public void Update(ReadOnlySpan<byte> bytes) =>
        _seed = DigestOf(_table, bytes, _seed);

    public void Update(byte[] bytes, int index, int length)
    {
        Validate(bytes, index, length);
        _seed = DigestOf(_table, bytes, index, length, _seed);
    }

    public uint Digest() => _seed;

    public byte[] DigestBytes() => BitConverter.GetBytes(_seed);

    //public HashAlgorithm AsHashAlgorithm() =>
    //    new HashAlgorithmAdapter(sizeof(uint) << 3, Reset, Update, DigestBytes);

    protected static void Validate(byte[] bytes, int offset, int length)
    {
        if (bytes == null || offset < 0 || length < 0 || offset + length > bytes.Length)
            throw new ArgumentException("Invalid buffer boundaries");
    }
}