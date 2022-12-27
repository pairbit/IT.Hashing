using System;
using System.Collections.Generic;

namespace IT.Hashing;

public interface ISpanHasher : IHashInformer
{
    IReadOnlyCollection<string> Algs { get; }

    bool TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash);

    bool TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? alg);
}