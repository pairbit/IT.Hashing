using System;

namespace IT.Hashing;

public interface ISpanHasher : IHashInformer
{
    bool TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? name = null);
}