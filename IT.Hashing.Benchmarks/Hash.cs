using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using IT.Hashing.Gost;
using IT.Hashing.Gost.Native;
using Org.BouncyCastle.Crypto;
using System.Buffers.Binary;

namespace IT.Hashing.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class HashBenchmark
{
    private static readonly Org.BouncyCastle.Crypto.Digests.Gost3411Digest digest94 = new();
    private static readonly Gost3411_2012_512Digest digest512 = new();
    private static readonly Gost3411_2012_256Digest digest256 = new();
    private static readonly Random _random = new();
    private byte[] _bytes = null!;

    [Params(1024)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var bytes = new byte[Length];

        _random.NextBytes(bytes);

        _bytes = bytes;
    }

    #region CRC32

    //[Benchmark]
    public uint IT_UInt32_CRC32() => CRC32.Hash32(_bytes);

    //[Benchmark]
    public uint IO_UInt32_CRC32() => BinaryPrimitives.ReadUInt32LittleEndian(System.IO.Hashing.Crc32.Hash(_bytes!));

    //[Benchmark]
    public uint Force_UInt32_CRC32() => Force.Crc32.Crc32Algorithm.Compute(_bytes);

    //[Benchmark]
    public byte[] IT_Bytes_CRC32()
    {
        var hash = new byte[4];

        BinaryPrimitives.WriteUInt32LittleEndian(hash, CRC32.Hash32(_bytes));

        return hash;
    }

    //[Benchmark]
    public byte[] IO_Bytes_CRC32() => System.IO.Hashing.Crc32.Hash(_bytes!);

    #endregion CRC32

    #region XXH32

    [Benchmark]
    public uint IT_UInt32_XXH32() => XXH32.Hash32(_bytes);

    [Benchmark]
    public uint IO_UInt32_XXH32() => System.IO.Hashing.XxHash32.HashToUInt32(_bytes);

#if NET6_0_OR_GREATER

    [Benchmark]
    public ulong ST_UInt32_XXH32() => Standart.Hash.xxHash.xxHash32.ComputeHash(_bytes);
#endif

    //[Benchmark]
    public byte[] IT_Bytes_XXH32()
    {
        var hash = new byte[4];

        BinaryPrimitives.WriteUInt32BigEndian(hash, XXH32.Hash32(_bytes));

        return hash;
    }

    //[Benchmark]
    public byte[] IO_Bytes_XXH32() => System.IO.Hashing.XxHash32.Hash(_bytes);

    #endregion XXH32

    #region XXH64

    [Benchmark]
    public ulong IT_UInt64_XXH64() => XXH64.Hash64(_bytes);

    [Benchmark]
    public ulong IO_UInt64_XXH64() => System.IO.Hashing.XxHash64.HashToUInt64(_bytes);

#if NET6_0_OR_GREATER

    [Benchmark]
    public ulong ST_UInt64_XXH64() => Standart.Hash.xxHash.xxHash64.ComputeHash(_bytes);
#endif

    //[Benchmark]
    public byte[] IT_Bytes_XXH64()
    {
        var hash = new byte[8];

        BinaryPrimitives.WriteUInt64BigEndian(hash, XXH64.Hash64(_bytes));

        return hash;
    }

    //[Benchmark]
    public byte[] IO_Bytes_XXH64() => System.IO.Hashing.XxHash64.Hash(_bytes);

    #endregion XXH64

    #region XXH3

    [Benchmark]
    public ulong IO_UInt64_XXH3() => System.IO.Hashing.XxHash3.HashToUInt64(_bytes);

#if NET6_0_OR_GREATER

    /// <summary>
    /// Win!!
    /// </summary>
    [Benchmark]
    public ulong ST_UInt64_XXH3() => Standart.Hash.xxHash.xxHash3.ComputeHash(_bytes, _bytes.Length);
#endif

    #endregion XXH3

    #region GOST

    //[Benchmark]
    public byte[] IT_GOST_256_Native()
    {
        using var alg = new Gost_R3411_2012_256_HashAlgorithm();
        return alg.ComputeHash(_bytes);
    }

    //[Benchmark]
    public byte[] IT_GOST_512_Native()
    {
        using var alg = new Gost_R3411_2012_512_HashAlgorithm();
        return alg.ComputeHash(_bytes);
    }

    //[Benchmark]
    public byte[] IT_GOST_94_Native()
    {
        using var alg = new Gost_R3411_94_HashAlgorithm();
        return alg.ComputeHash(_bytes);
    }

    //[Benchmark]
    public byte[] IT_GOST_256() => CalculateDigest(digest256, _bytes);

    //[Benchmark]
    public byte[] IT_GOST_512() => CalculateDigest(digest512, _bytes);

    //[Benchmark]
    public byte[] GOST_94() => CalculateDigest(digest94, _bytes);

    #endregion

    private static byte[] CalculateDigest(Gost3411_2012Digest digest, byte[] input)
    {
        digest.BlockUpdate(input, 0, input.Length);

        byte[] b = new byte[digest.GetDigestSize()];

        digest.DoFinal(b, 0);

        return b;
    }

    private static byte[] CalculateDigest(IDigest digest, byte[] input)
    {
        digest.BlockUpdate(input, 0, input.Length);

        byte[] b = new byte[digest.GetDigestSize()];

        digest.DoFinal(b, 0);

        return b;
    }
}