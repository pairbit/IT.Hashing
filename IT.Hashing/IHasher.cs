using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher : ISpanHasher
{
    IHashAlg GetAlg(string? name = null);

    bool TryHash(Stream stream, Span<byte> hash, string? name = null);

    Task<bool> TryHashAsync(Stream stream, Span<byte> hash, string? name = null, CancellationToken token = default);
}