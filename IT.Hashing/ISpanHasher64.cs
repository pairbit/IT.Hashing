using System;

namespace IT.Hashing;

public interface ISpanHasher64 : ISpanHasher
{
    ulong Hash64(string alg, ReadOnlySpan<byte> bytes);
}