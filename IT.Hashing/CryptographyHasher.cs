//using System;
//using System.Buffers;
//using System.Collections.Generic;
//using System.IO;
//using System.Security.Cryptography;
//using System.Threading;
//using System.Threading.Tasks;

//namespace IT.Hashing;

//public class CryptographyHasher : IHasher
//{
//    private List<HashInfo> infos = new List<HashInfo>()
//    {
//        new("MD5","MD5",1,1,"1.2.840.113549.2.5"),
//        new("SHA1","SHA1",1,1,"1.3.14.3.2.26"),
//        new("SHA256","MD5",1,1,"2.16.840.1.101.3.4.2.1"),
//        new("SHA384","MD5",1,1,"2.16.840.1.101.3.4.2.2"),
//        new("SHA512","SHA512",1,1,"2.16.840.1.101.3.4.2.3"),
//    };

//    #region IHashInformer

//    public IReadOnlyCollection<HashInfo> Info => infos;

//    #endregion IHashInformer



//    #region ISpanHasher

//    public int TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? name)
//    {
//#if NET6_0_OR_GREATER
//        if (name == "MD5") return MD5.TryHashData(bytes, hash, out var written) ? written : 0;
//        if (name == "SHA1") return SHA1.TryHashData(bytes, hash, out var written) ? written : 0;
//        if (name == "SHA256") return SHA256.TryHashData(bytes, hash, out var written) ? written : 0;
//        if (name == "SHA384") return SHA384.TryHashData(bytes, hash, out var written) ? written : 0;
//        if (name == "SHA512") return SHA512.TryHashData(bytes, hash, out var written) ? written : 0;

//        throw new ArgumentException($"Alg '{name}' not found", nameof(name));
//#else
//        using var alg = TryCreate(name);

//        if (alg.HashSize < hash.Length) return 0;

//        var pool = ArrayPool<byte>.Shared;

//        byte[] buffer = pool.Rent(bytes.Length);

//        bytes.CopyTo(buffer);

//        var computedHash = alg.ComputeHash(buffer);

//        if (computedHash.Length < hash.Length) return 0;

//        computedHash.CopyTo(hash);

//        pool.Return(buffer);

//        return computedHash.Length;
//#endif
//    }

//    #endregion ISpanHasher

//    #region IHasher

//    public IHashAlg GetAlg(string? name)
//    {
//        throw new NotImplementedException();
//    }

//    public int TryHash(Stream stream, Span<byte> hash, string? name)
//    {
//#if NET7_0_OR_GREATER
//        if (name == "MD5") return hash.Length < MD5.HashSizeInBytes ? 0 : MD5.HashData(stream, hash);
//        if (name == "SHA1") return hash.Length < SHA1.HashSizeInBytes ? 0 : SHA1.HashData(stream, hash);
//        if (name == "SHA256") return hash.Length < SHA256.HashSizeInBytes ? 0 : SHA256.HashData(stream, hash);
//        if (name == "SHA384") return hash.Length < SHA384.HashSizeInBytes ? 0 : SHA384.HashData(stream, hash);
//        if (name == "SHA512") return hash.Length < SHA512.HashSizeInBytes ? 0 : SHA512.HashData(stream, hash);

//        throw new ArgumentException($"Alg '{name}' not found", nameof(name));
//#else
//        using var alg = TryCreate(name);

//        if (alg.HashSize < hash.Length) return 0;

//        var computedHash = alg.ComputeHash(stream);

//        if (computedHash.Length < hash.Length) return 0;

//        computedHash.CopyTo(hash);

//        return computedHash.Length;
//#endif
//    }

//#if NET7_0_OR_GREATER

//    public ValueTask<int> TryHashAsync(Stream stream, Memory<byte> hash, string? name, CancellationToken token = default)
//    {
//        if (name == "MD5") return hash.Length < MD5.HashSizeInBytes ? new ValueTask<int>(0) : MD5.HashDataAsync(stream, hash, token);
//        if (name == "SHA1") return hash.Length < SHA1.HashSizeInBytes ? new ValueTask<int>(0) : SHA1.HashDataAsync(stream, hash, token);
//        if (name == "SHA256") return hash.Length < SHA256.HashSizeInBytes ? new ValueTask<int>(0) : SHA256.HashDataAsync(stream, hash, token);
//        if (name == "SHA384") return hash.Length < SHA384.HashSizeInBytes ? new ValueTask<int>(0) : SHA384.HashDataAsync(stream, hash, token);
//        if (name == "SHA512") return hash.Length < SHA512.HashSizeInBytes ? new ValueTask<int>(0) : SHA512.HashDataAsync(stream, hash, token);

//        return ValueTask.FromException<int>(new ArgumentException($"Alg '{name}' not found", nameof(name)));
//    }

//#else

//    public async ValueTask<int> TryHashAsync(Stream stream, Memory<byte> hash, string? name, CancellationToken token = default)
//    {
//        using var alg = TryCreate(name);

//        if (alg.HashSize < hash.Length) return 0;

//#if NET6_0_OR_GREATER
//        var computedHash = await alg.ComputeHashAsync(stream, token);
//#else
//        var pool = ArrayPool<byte>.Shared;

//        byte[] buffer = pool.Rent(4096);

//        int readed;

//        while ((readed = await stream.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false)) > 0)
//            alg.TransformBlock(buffer, 0, readed, buffer, 0);

//        alg.TransformFinalBlock(buffer, 0, readed);

//        pool.Return(buffer);

//        var computedHash = alg.Hash;
//#endif

//        if (computedHash.Length < hash.Length) return 0;

//        computedHash.CopyTo(hash);

//        return computedHash.Length;
//    }
//#endif

//    #endregion IHasher

//    private static HashAlgorithm? TryCreate(String name)
//    {
//        if (name == "MD5") return MD5.Create();
//        if (name == "SHA1") return SHA1.Create();
//        if (name == "SHA256") return SHA256.Create();
//        if (name == "SHA384") return SHA384.Create();
//        if (name == "SHA512") return SHA512.Create();

//        var crypto = CryptoConfig.CreateFromName(name);

//        if (crypto == null) return null;

//        if (crypto is HashAlgorithm hashAlgorithm) return hashAlgorithm;

//        if (crypto is SignatureDescription signatureDescription) return signatureDescription.CreateDigest();

//        throw new InvalidOperationException($"Hash algorithm '{name}' not supported '{crypto.GetType().FullName}'");
//    }

//    private static HashAlgorithm Create(String name)
//    {
//        var hasher = TryCreate(name);

//        if (hasher is null) throw new ArgumentException($"Hash algorithm '{name}' not found.", nameof(name));

//        return hasher!;
//    }
//}