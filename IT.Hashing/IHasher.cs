using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher : ISpanHasher
{
    IHashAlg GetAlg(string? name = null);

    int TryHash(Stream stream, Span<byte> hash, string? name = null);

    ValueTask<int> TryHashAsync(Stream stream, Memory<byte> hash, string? name = null, CancellationToken token = default);
}