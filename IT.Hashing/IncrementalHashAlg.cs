using System;
using System.Security.Cryptography;

namespace IT.Hashing;

public class IncrementalHashAlg : IHashAlg
{
    private readonly IncrementalHash _incrementalHash;
    private readonly HashInfo _hashInfo;

    public HashInfo Info => _hashInfo;

    public IncrementalHashAlg(HashAlgorithmName algorithmName, HashInfo hashInfo)
    {
        _incrementalHash = IncrementalHash.CreateHash(algorithmName);
        _hashInfo = hashInfo ?? throw new ArgumentNullException(nameof(hashInfo));
    }

    public IncrementalHashAlg(IncrementalHash incrementalHash, HashInfo hashInfo)
    {
        _incrementalHash = incrementalHash ?? throw new ArgumentNullException(nameof(incrementalHash));
        _hashInfo = hashInfo ?? throw new ArgumentNullException(nameof(hashInfo));
    }

    public void Dispose() => _incrementalHash.Dispose();

#if NETSTANDARD2_0

    public void Append(ReadOnlySpan<byte> bytes)
    {
        var pool = System.Buffers.ArrayPool<byte>.Shared;

        byte[] rented = pool.Rent(bytes.Length);

        bytes.CopyTo(rented);

        _incrementalHash.AppendData(rented, 0, bytes.Length);

        Array.Clear(rented, 0, bytes.Length);

        pool.Return(rented);
    }

    public void Append(byte[] bytes) => _incrementalHash.AppendData(bytes);

    public void Append(byte[] bytes, int offset, int count) => _incrementalHash.AppendData(bytes, offset, count);

    public void Append(System.IO.Stream stream) => Internal.StreamHasher.Append(stream, Append);

    public System.Threading.Tasks.Task AppendAsync(System.IO.Stream stream, System.Threading.CancellationToken token = default) => Internal.StreamHasher.AppendAsync(stream, Append, token);

    public void Reset() => _incrementalHash.GetHashAndReset();

    public int TryGetHash(Span<byte> hash)
    {
        if (hash.Length < Info.SizeInBytes) return 0;

        var hashArray = _incrementalHash.GetHashAndReset();

        hashArray.CopyTo(hash);

        return hashArray.Length;
    }

    public byte[] GetHash() => _incrementalHash.GetHashAndReset();

#else

    public void Append(ReadOnlySpan<byte> bytes) => _incrementalHash.AppendData(bytes);

    public void Reset() => _incrementalHash.TryGetHashAndReset(stackalloc byte[Info.SizeInBytes], out _);

#if NET6_0_OR_GREATER

    public int TryGetHash(Span<byte> hash) => _incrementalHash.TryGetCurrentHash(hash, out int bytesWritten) ? bytesWritten : 0;

#else

    public int TryGetHash(Span<byte> hash) => _incrementalHash.TryGetHashAndReset(hash, out int bytesWritten) ? bytesWritten : 0;

#endif

#endif
}