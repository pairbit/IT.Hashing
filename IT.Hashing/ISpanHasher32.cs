using System;

namespace IT.Hashing;

public interface ISpanHasher32 : ISpanHasher
{
    uint Hash32(string alg, ReadOnlySpan<byte> bytes);
}