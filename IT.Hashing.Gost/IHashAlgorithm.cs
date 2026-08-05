using System;

namespace IT.Hashing.Gost;

public interface IHashAlgorithm : IDisposable
{
    int Size { get; }

    void Append(byte value);

    void Append(ReadOnlySpan<byte> span);

    void Append(byte[] array, int start, int length);

    bool TryGetHash(Span<byte> hash, out int written);

    void Reset();
}