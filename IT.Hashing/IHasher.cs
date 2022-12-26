using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher : ISpanHasher, IHashInformer
{
    bool TryHash(string alg, Stream stream, Span<byte> hash);

    Task<bool> TryHashAsync(string alg, Stream stream, Span<byte> hash, CancellationToken token);
}