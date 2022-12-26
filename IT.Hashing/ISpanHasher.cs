using System;

namespace IT.Hashing;

public interface ISpanHasher
{
    bool TryHash(string alg, ReadOnlySpan<byte> bytes, Span<byte> hash);
}