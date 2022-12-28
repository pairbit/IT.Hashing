using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public class CryptographyHasher : IHasher
{
    private List<HashInfo> infos = new List<HashInfo>()
    {
        new("MD5","MD5",1,1,"1.2.840.113549.2.5"),
        new("SHA1","SHA1",1,1,"1.3.14.3.2.26"),
        new("SHA256","MD5",1,1,"2.16.840.1.101.3.4.2.1"),
        new("SHA384","MD5",1,1,"2.16.840.1.101.3.4.2.2"),
        new("SHA512","SHA512",1,1,"2.16.840.1.101.3.4.2.3"),
    };

    #region IHashInformer

    public IReadOnlyCollection<HashInfo> Info => infos;

    #endregion IHashInformer



    #region ISpanHasher

    public bool TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? name)
    {
        using var alg = TryCreate(name);

#if NETSTANDARD2_0
        if (alg.HashSize < hash.Length) return false;

        var pool = ArrayPool<byte>.Shared;

        byte[] buffer = pool.Rent(bytes.Length);

        bytes.CopyTo(buffer);

        var computedHash = alg.ComputeHash(buffer);

        if (computedHash.Length < hash.Length) return false;

        computedHash.CopyTo(hash);

        pool.Return(buffer);

        return true;
#else
        return alg.TryComputeHash(bytes, hash, out _);
#endif
    }

    #endregion ISpanHasher

    #region IHasher

    public IHashAlg GetAlg(string? name)
    {
        throw new NotImplementedException();
    }

    public bool TryHash(Stream stream, Span<byte> hash, string? name)
    {
        using var alg = TryCreate(name);

        if (alg.HashSize < hash.Length) return false;

        var computedHash = alg.ComputeHash(stream);

        if (computedHash.Length < hash.Length) return false;

        computedHash.CopyTo(hash);

        return true;
    }

    public async Task<bool> TryHashAsync(Stream stream, Memory<byte> hash, string? name, CancellationToken token = default)
    {
        using var alg = TryCreate(name);

        if (alg.HashSize < hash.Length) return false;

#if NET6_0_OR_GREATER
        var computedHash = await alg.ComputeHashAsync(stream, token);
#else
        var pool = ArrayPool<byte>.Shared;

        byte[] buffer = pool.Rent(4096);

        int readed;

        while ((readed = await stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false)) > 0)
            alg.TransformBlock(buffer, 0, readed, buffer, 0);

        alg.TransformFinalBlock(buffer, 0, readed);

        pool.Return(buffer);

        var computedHash = alg.Hash;

#endif
        if (computedHash.Length < hash.Length) return false;

        computedHash.CopyTo(hash);

        return true;
    }

#endregion IHasher

    private static HashAlgorithm? TryCreate(String name)
    {
        var crypto = CryptoConfig.CreateFromName(name);

        if (crypto == null) return null;

        if (crypto is HashAlgorithm hashAlgorithm) return hashAlgorithm;

        if (crypto is SignatureDescription signatureDescription) return signatureDescription.CreateDigest();

        throw new InvalidOperationException($"Hash algorithm '{name}' not supported '{crypto.GetType().FullName}'");
    }

    private static HashAlgorithm Create(String name)
    {
        var hasher = TryCreate(name);

        if (hasher is null) throw new ArgumentException($"Hash algorithm '{name}' not found.", nameof(name));

        return hasher!;
    }
}