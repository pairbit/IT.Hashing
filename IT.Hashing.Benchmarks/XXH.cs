using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using System.Buffers.Binary;

namespace IT.Hashing.Benchmarks;

[MemoryDiagnoser]
[MinColumn, MaxColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest, MethodOrderPolicy.Declared)]
public class XXHBenchmark
{
    private static readonly Random _random = new();
    private byte[]? _bytes;

    [Params(1024 * 1024)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var bytes = new byte[Length];

        _random.NextBytes(bytes);

        _bytes = bytes;
    }

    #region XXH32

    [Benchmark]
    public uint IT_UInt32_XXH32() => XXH32.DigestOf(_bytes);

    [Benchmark]
    public uint IO_UInt32_XXH32() => BinaryPrimitives.ReadUInt32BigEndian(System.IO.Hashing.XxHash32.Hash(_bytes!));

    [Benchmark]
    public byte[] IT_Bytes_XXH32()
    {
        var hash = new byte[4];

        BinaryPrimitives.WriteUInt32BigEndian(hash, XXH32.DigestOf(_bytes));

        return hash;
    }

    [Benchmark]
    public byte[] IO_Bytes_XXH32() => System.IO.Hashing.XxHash32.Hash(_bytes!);

    #endregion XXH32

    #region XXH64

    [Benchmark]
    public ulong IT_UInt64_XXH64() => XXH64.DigestOf(_bytes);

    [Benchmark]
    public ulong IO_UInt64_XXH64() => BinaryPrimitives.ReadUInt64BigEndian(System.IO.Hashing.XxHash64.Hash(_bytes!));

    [Benchmark]
    public byte[] IT_Bytes_XXH64()
    {
        var hash = new byte[8];

        BinaryPrimitives.WriteUInt64BigEndian(hash, XXH64.DigestOf(_bytes));

        return hash;
    }

    [Benchmark]
    public byte[] IO_Bytes_XXH64() => System.IO.Hashing.XxHash64.Hash(_bytes!);

    #endregion XXH64
}