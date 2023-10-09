using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher64 : IHasher
{
    IHashAlg64 GetAlg64(string? name = null);

    ulong Hash64(ReadOnlySpan<byte> bytes, string? name = null);

    ulong Hash64(Stream stream, string? name = null);

    ValueTask<ulong> Hash64Async(Stream stream, string? name = null, CancellationToken token = default);
}