using System;

namespace IT.Hashing;

public interface ISpanHasher32 : ISpanHasher
{
    uint Hash32(ReadOnlySpan<byte> bytes, string? name = null);
}