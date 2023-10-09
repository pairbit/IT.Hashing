using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher32 : IHasher
{
    IHashAlg32 GetAlg32(string? name = null);

    uint Hash32(ReadOnlySpan<byte> bytes, string? name = null);

    uint Hash32(Stream stream, string? name = null);

    ValueTask<uint> Hash32Async(Stream stream, string? name = null, CancellationToken token = default);
}