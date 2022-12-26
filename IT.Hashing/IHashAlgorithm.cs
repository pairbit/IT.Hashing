﻿using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace IT.Hashing;

public interface IHashAlgorithm
{
    string? Oid { get; }

    int Size { get; }

    void Append(ReadOnlySpan<byte> bytes);

    void Append(Stream stream);

    Task AppendAsync(Stream stream, CancellationToken cancellationToken = default);

    void Reset();

    bool TryHash(Span<byte> hash);
}