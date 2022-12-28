using System;

namespace IT.Hashing;

public interface ISpanHasher : IHashInformer
{
    int TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? name = null);
}