using System;

namespace IT.Hashing;

public interface ISpanHasher : IHashInformer
{
    bool TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash);

    bool TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? alg);
}