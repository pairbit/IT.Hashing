using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher : IHashInformer
{
    IHashAlg GetAlg(string? name = null);

    byte[] Hash(byte[] bytes, string? name = null);

    byte[] Hash(byte[] bytes, int offset, int count, string? name = null);

    int TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? name = null);

    int Hash(Stream stream, Span<byte> hash, string? name = null);

    ValueTask<int> HashAsync(Stream stream, Memory<byte> hash, string? name = null, CancellationToken token = default);
}