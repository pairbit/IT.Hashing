using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace IT.Hashing;

public interface IHashAlg
{
    HashInfo Info { get; }

    void Append(ReadOnlySpan<byte> bytes);

    void Append(Stream stream);

    Task AppendAsync(Stream stream, CancellationToken token = default);

    void Reset();

    bool TryHash(Span<byte> hash);
}