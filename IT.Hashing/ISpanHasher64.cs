using System;

namespace IT.Hashing;

public interface ISpanHasher64 : ISpanHasher
{
    ulong Hash64(ReadOnlySpan<byte> bytes, string? name = null);
}