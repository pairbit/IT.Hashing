using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher : ISpanHasher
{
    IHashAlgorithm GetAlgorithm();

    IHashAlgorithm GetAlgorithm(string? alg);

    bool TryHash(Stream stream, Span<byte> hash);

    bool TryHash(Stream stream, Span<byte> hash, string? alg);

    Task<bool> TryHashAsync(Stream stream, Span<byte> hash, CancellationToken token = default);

    Task<bool> TryHashAsync(Stream stream, Span<byte> hash, string? alg, CancellationToken token = default);
}