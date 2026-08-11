using System;

namespace IT.Hashing.Gost;

public interface IHashAlgorithm : IDisposable
{
    int Size { get; }

    int SizeInBase64 { get; }

    void Append(byte value);

    void Append(ReadOnlySpan<byte> span);

    void Append(byte[] array, int start, int length);

    bool TryGetHash(Span<byte> hash, out int length);

    bool TryGetHashInBase64(Span<byte> hash, out int length);

    void Reset();
}