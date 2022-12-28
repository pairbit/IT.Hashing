using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public class Hasher : IHasher
{
    #region IHashInformer

    public IReadOnlyCollection<HashInfo> Info => throw new NotImplementedException();

    #endregion IHashInformer

    #region ISpanHasher

    public bool TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? name)
    {
        throw new NotImplementedException();
    }

    #endregion ISpanHasher

    #region IHasher

    public IHashAlg GetAlg(string? name)
    {
        throw new NotImplementedException();
    }

    public bool TryHash(Stream stream, Span<byte> hash, string? name)
    {
        throw new NotImplementedException();
    }

    public Task<bool> TryHashAsync(Stream stream, Span<byte> hash, string? name, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    #endregion IHasher
}