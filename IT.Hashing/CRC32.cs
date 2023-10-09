using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

//https://github.com/force-net/Crc32.NET
public class CRC32 : IHashAlg32
{
    //polynomial
    private const uint Poly = 0xedb88320u;
    private static readonly uint[] _table = Build(Poly);
    private uint _crc;

    public const int HashSizeInBits = 32;
    public const int HashSizeInBytes = HashSizeInBits / 8;
    public static readonly HashInfo HashInfo = new(typeof(CRC32).FullName!, "CRC32", HashSizeInBytes, null);

    public HashInfo Info => HashInfo;

    public CRC32()
    {

    }

    public static HashAlgorithm Create() => new Internal.HashAlgorithmAdapter(new XXH32());

    public static int TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash)
    {
        if (hash.Length < sizeof(uint)) return 0;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(hash), Hash32(bytes));

        return HashSizeInBytes;
    }

    public static byte[] Hash(ReadOnlySpan<byte> bytes)
    {
        byte[] hash = new byte[sizeof(uint)];
        Unsafe.As<byte, uint>(ref hash[0]) = Hash32(bytes);
        return hash;
    }

    public static int Hash(Stream stream, Span<byte> hash) => throw new NotImplementedException();

    public static ValueTask<int> HashAsync(Stream stream, Memory<byte> hash, CancellationToken token = default)
        => throw new NotImplementedException();

    #region HashAlgorithm

    //protected override void HashCore(byte[] input, int offset, int length)
    //{
    //    if (length > 0) _crc = Append(new ReadOnlySpan<byte>(input, offset, length), _crc);
    //}

    //protected override byte[] HashFinal()
    //{
    //    return new[] { (byte)(_crc >> 24), (byte)(_crc >> 16), (byte)(_crc >> 8), (byte)_crc };
    //}

    #endregion HashAlgorithm

    #region IHashAlg32

    public uint GetHash32() => _crc;

    public void Append(ReadOnlySpan<byte> bytes) => _crc = Append(bytes, _crc);

#if NETSTANDARD2_0

    public void Append(byte[] bytes) => Append(bytes.AsSpan());

    public void Append(byte[] bytes, int offset, int count) => Append(bytes.AsSpan(offset, count));

    public void Append(Stream stream) => Internal.StreamHasher.Append(stream, Append);

    public Task AppendAsync(Stream stream, CancellationToken token = default) => Internal.StreamHasher.AppendAsync(stream, Append, token);

    public byte[] GetHash()
    {
        var bytes = new byte[sizeof(uint)];
        Unsafe.As<byte, uint>(ref bytes[0]) = _crc;
        return bytes;
    }

#endif

    public void Reset() => _crc = 0;

    public int TryGetHash(Span<byte> hash)
    {
        if (hash.Length < sizeof(uint)) return 0;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(hash), _crc);

        return sizeof(uint);
    }

    public void Dispose() { }

    #endregion IHashAlg32

    public static uint Hash32(ReadOnlySpan<byte> bytes) => Append(bytes, 0);

    public static uint Hash32(Stream stream) => throw new NotImplementedException();

    public static ValueTask<uint> Hash32Async(Stream stream, CancellationToken token = default) => throw new NotImplementedException();

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

    public static uint Append(ReadOnlySpan<byte> bytes, uint crc)
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