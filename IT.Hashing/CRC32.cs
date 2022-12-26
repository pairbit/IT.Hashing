using System;
using System.Security.Cryptography;

namespace IT.Hashing;

//https://github.com/force-net/Crc32.NET
public class CRC32 : HashAlgorithm
{
    //polynomial
    private const uint Poly = 0xedb88320u;
    private static readonly uint[] _table = Build(Poly);
    private uint _crc;

    public CRC32()
    {
        HashSizeValue = 32;
    }

    #region HashAlgorithm

    public override void Initialize()
    {
        _crc = 0;
    }

    protected override void HashCore(byte[] input, int offset, int length)
    {
        if (length > 0) _crc = AppendInternal(new ReadOnlySpan<byte>(input, offset, length), _crc);
    }

    protected override byte[] HashFinal()
    {
        return new[] { (byte)(_crc >> 24), (byte)(_crc >> 16), (byte)(_crc >> 8), (byte)_crc };
    }

    #endregion HashAlgorithm

    public static uint Append(ReadOnlySpan<byte> bytes, uint initial) => AppendInternal(bytes, initial);

    public static uint DigestOf(ReadOnlySpan<byte> bytes) => AppendInternal(bytes, 0);

    protected static uint[] Build(uint poly)
    {
        var table = new uint[16 * 256];
        for (uint i = 0; i < 256; i++)
        {
            uint res = i;
            for (int t = 0; t < 16; t++)
            {
                for (int k = 0; k < 8; k++) res = (res & 1) == 1 ? poly ^ (res >> 1) : (res >> 1);
                table[(t * 256) + i] = res;
            }
        }
        return table;
    }

    protected static uint AppendInternal(ReadOnlySpan<byte> bytes, uint crc)
    {
        crc = uint.MaxValue ^ crc;
        var length = bytes.Length;
        uint[] table = _table;
        var offset = 0;
        while (length >= 16)
        {
            var a = table[(3 * 256) + bytes[offset + 12]]
                ^ table[(2 * 256) + bytes[offset + 13]]
                ^ table[(1 * 256) + bytes[offset + 14]]
                ^ table[(0 * 256) + bytes[offset + 15]];

            var b = table[(7 * 256) + bytes[offset + 8]]
                ^ table[(6 * 256) + bytes[offset + 9]]
                ^ table[(5 * 256) + bytes[offset + 10]]
                ^ table[(4 * 256) + bytes[offset + 11]];

            var c = table[(11 * 256) + bytes[offset + 4]]
                ^ table[(10 * 256) + bytes[offset + 5]]
                ^ table[(9 * 256) + bytes[offset + 6]]
                ^ table[(8 * 256) + bytes[offset + 7]];

            var d = table[(15 * 256) + ((byte)crc ^ bytes[offset])]
                ^ table[(14 * 256) + ((byte)(crc >> 8) ^ bytes[offset + 1])]
                ^ table[(13 * 256) + ((byte)(crc >> 16) ^ bytes[offset + 2])]
                ^ table[(12 * 256) + ((crc >> 24) ^ bytes[offset + 3])];

            crc = d ^ c ^ b ^ a;
            offset += 16;
            length -= 16;
        }

        while (--length >= 0)
            crc = table[(byte)(crc ^ bytes[offset++])] ^ crc >> 8;

        return crc ^ uint.MaxValue;
    }
}