using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHashAlg : IDisposable
{
    HashInfo Info { get; }

    void Append(ReadOnlySpan<byte> bytes);

    void Reset();

    int TryGetHash(Span<byte> hash);

#if NETSTANDARD2_0

    void Append(byte[] bytes);

    void Append(byte[] bytes, int offset, int count);

    void Append(Stream stream);

    Task AppendAsync(Stream stream, CancellationToken token = default);

    byte[] GetHash();

#else

    void Append(byte[] bytes) => Append(bytes.AsSpan());

    void Append(byte[] bytes, int offset, int count) => Append(bytes.AsSpan(offset, count));

    void Append(Stream stream) => Internal.StreamHasher.Append(stream, Append);

    Task AppendAsync(Stream stream, CancellationToken token = default) => Internal.StreamHasher.AppendAsync(stream, Append, token);

    byte[] GetHash()
    {
        var hash = new byte[Info.SizeInBytes];
        TryGetHash(hash);
        return hash;
    }

#endif
}