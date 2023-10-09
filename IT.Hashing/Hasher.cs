using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public class Hasher : IHasher, IHasher32, IHasher64
{
    private readonly string _hashNameDefault;

    private static class HashInfos
    {
        public readonly static HashInfo MD5 = new("System.Security.Cryptography.MD5", "MD5", 16, "1.2.840.113549.2.5");
        public readonly static HashInfo SHA1 = new("System.Security.Cryptography.SHA1", "SHA1", 20, "1.3.14.3.2.26");
        public readonly static HashInfo SHA256 = new("System.Security.Cryptography.SHA256", "SHA256", 32, "2.16.840.1.101.3.4.2.1");
        public readonly static HashInfo SHA384 = new("System.Security.Cryptography.SHA384", "SHA384", 48, "2.16.840.1.101.3.4.2.2");
        public readonly static HashInfo SHA512 = new("System.Security.Cryptography.SHA512", "SHA512", 64, "2.16.840.1.101.3.4.2.3");
    }

    public static readonly IReadOnlyList<HashInfo> Infos = new ReadOnlyCollection<HashInfo>(new List<HashInfo> {
        XXH32.HashInfo, XXH64.HashInfo, CRC32.HashInfo,
        HashInfos.MD5, HashInfos.SHA1, HashInfos.SHA256, HashInfos.SHA384, HashInfos.SHA512
    });

    public Hasher(string hashNameDefault = "SHA256")
    {
        _hashNameDefault = hashNameDefault;
    }

    public IReadOnlyList<HashInfo> Info => Infos;

    public IHashAlg GetAlg(string? name)
    {
        name ??= _hashNameDefault;

        if (name == "XXH32") return new XXH32();
        if (name == "XXH64") return new XXH64();
        if (name == "CRC32") return new CRC32();
        if (name == "MD5") return new IncrementalHashAlg(HashAlgorithmName.MD5, HashInfos.MD5);
        if (name == "SHA1") return new IncrementalHashAlg(HashAlgorithmName.SHA1, HashInfos.SHA1);
        if (name == "SHA256") return new IncrementalHashAlg(HashAlgorithmName.SHA256, HashInfos.SHA256);
        if (name == "SHA384") return new IncrementalHashAlg(HashAlgorithmName.SHA384, HashInfos.SHA384);
        if (name == "SHA512") return new IncrementalHashAlg(HashAlgorithmName.SHA512, HashInfos.SHA512);

        throw new ArgumentException($"Alg '{name}' not found", nameof(name));
    }

    public IHashAlg32 GetAlg32(string? name)
    {
        if (name == null || name == "XXH32") return new XXH32();
        if (name == "CRC32") return new CRC32();

        throw new ArgumentException($"Alg32 '{name}' not found", nameof(name));
    }

    public IHashAlg64 GetAlg64(string? name)
    {
        if (name == null || name == "XXH64") return new XXH64();
        //if (name == "CRC64") return new CRC64();

        throw new ArgumentException($"Alg64 '{name}' not found", nameof(name));
    }

    public byte[] Hash(byte[] bytes, string? name = null)
    {
        if (bytes == null) throw new ArgumentNullException(nameof(bytes));

        var span = new ReadOnlySpan<byte>(bytes);

        name ??= _hashNameDefault;

        if (name == "XXH32") return XXH32.Hash(span);
        if (name == "XXH64") return XXH64.Hash(span);
        if (name == "CRC32") return CRC32.Hash(span);

#if NET6_0_OR_GREATER
        if (name == "MD5") return MD5.HashData(span);
        if (name == "SHA1") return SHA1.HashData(span);
        if (name == "SHA256") return SHA256.HashData(span);
        if (name == "SHA384") return SHA384.HashData(span);
        if (name == "SHA512") return SHA512.HashData(span);

        throw new ArgumentException($"Alg '{name}' not found", nameof(name));
#else
        using var alg = GetAlg(name);

        alg.Append(bytes);

        return alg.GetHash();
#endif
    }

    public byte[] Hash(byte[] bytes, int offset, int count, string? name = null)
    {
        if (bytes == null) throw new ArgumentNullException(nameof(bytes));
        var len = bytes.Length;
        if (offset < 0 || offset > len) throw new ArgumentOutOfRangeException(nameof(offset));
        if (count < 0 || count > len) throw new ArgumentOutOfRangeException(nameof(count));
        if (len - count < offset) throw new ArgumentException();

        var span = new ReadOnlySpan<byte>(bytes, offset, count);

        if (name == "XXH32") return XXH32.Hash(span);
        if (name == "XXH64") return XXH64.Hash(span);
        if (name == "CRC32") return CRC32.Hash(span);

#if NET6_0_OR_GREATER
        if (name == "MD5") return MD5.HashData(span);
        if (name == "SHA1") return SHA1.HashData(span);
        if (name == "SHA256") return SHA256.HashData(span);
        if (name == "SHA384") return SHA384.HashData(span);
        if (name == "SHA512") return SHA512.HashData(span);

        throw new ArgumentException($"Alg '{name}' not found", nameof(name));
#else
        using var alg = GetAlg(name);

        alg.Append(bytes, offset, count);

        return alg.GetHash();
#endif
    }

    public int TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash, string? name)
    {
        name ??= _hashNameDefault;

        if (name == "XXH32") return XXH32.TryHash(bytes, hash);
        if (name == "XXH64") return XXH64.TryHash(bytes, hash);
        if (name == "CRC32") return CRC32.TryHash(bytes, hash);

#if NET6_0_OR_GREATER
        if (name == "MD5") return MD5.TryHashData(bytes, hash, out var written) ? written : 0;
        if (name == "SHA1") return SHA1.TryHashData(bytes, hash, out var written) ? written : 0;
        if (name == "SHA256") return SHA256.TryHashData(bytes, hash, out var written) ? written : 0;
        if (name == "SHA384") return SHA384.TryHashData(bytes, hash, out var written) ? written : 0;
        if (name == "SHA512") return SHA512.TryHashData(bytes, hash, out var written) ? written : 0;

        throw new ArgumentException($"Alg '{name}' not found", nameof(name));
#else
        using var alg = GetAlg(name);

        if (hash.Length < alg.Info.SizeInBytes) return 0;

        alg.Append(bytes);

        return alg.TryGetHash(hash);
#endif
    }

    public uint Hash32(ReadOnlySpan<byte> bytes, string? name)
    {
        if (name == null || name == "XXH32") return XXH32.Hash32(bytes);
        if (name == "CRC32") return CRC32.Hash32(bytes);

        throw new ArgumentException($"Alg32 '{name}' not found", nameof(name));
    }

    public ulong Hash64(ReadOnlySpan<byte> bytes, string? name)
    {
        if (name == null || name == "XXH64") return XXH64.Hash64(bytes);

        throw new ArgumentException($"Alg64 '{name}' not found", nameof(name));
    }

    public int Hash(Stream stream, Span<byte> hash, string? name)
    {
        name ??= _hashNameDefault;

        if (name == "XXH32") return XXH32.Hash(stream, hash);
        if (name == "XXH64") return XXH64.Hash(stream, hash);
        if (name == "CRC32") return CRC32.Hash(stream, hash);

#if NET7_0_OR_GREATER
        if (name == "MD5") return MD5.HashData(stream, hash);
        if (name == "SHA1") return SHA1.HashData(stream, hash);
        if (name == "SHA256") return SHA256.HashData(stream, hash);
        if (name == "SHA384") return SHA384.HashData(stream, hash);
        if (name == "SHA512") return SHA512.HashData(stream, hash);

        throw new ArgumentException($"Alg '{name}' not found", nameof(name));
#else
        if (stream == null) throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead) throw new ArgumentException("Stream not readable", nameof(stream));

        using var alg = GetAlg(name);

        if (hash.Length < alg.Info.SizeInBytes) throw new ArgumentException("Destination too small", nameof(hash));

        alg.Append(stream);

        return alg.TryGetHash(hash);
#endif
    }

    public uint Hash32(Stream stream, string? name)
    {
        if (name == null || name == "XXH32") return XXH32.Hash32(stream);
        if (name == "CRC32") return CRC32.Hash32(stream);

        throw new ArgumentException($"Alg32 '{name}' not found", nameof(name));
    }

    public ulong Hash64(Stream stream, string? name)
    {
        if (name == null || name == "XXH64") return XXH64.Hash64(stream);

        throw new ArgumentException($"Alg64 '{name}' not found", nameof(name));
    }

#if NET7_0_OR_GREATER

    public ValueTask<int> HashAsync(Stream stream, Memory<byte> hash, string? name, CancellationToken token = default)
    {
        name ??= _hashNameDefault;

        if (name == "XXH32") return XXH32.HashAsync(stream, hash, token);
        if (name == "XXH64") return XXH64.HashAsync(stream, hash, token);
        if (name == "CRC32") return CRC32.HashAsync(stream, hash, token);
        if (name == "MD5") return MD5.HashDataAsync(stream, hash, token);
        if (name == "SHA1") return SHA1.HashDataAsync(stream, hash, token);
        if (name == "SHA256") return SHA256.HashDataAsync(stream, hash, token);
        if (name == "SHA384") return SHA384.HashDataAsync(stream, hash, token);
        if (name == "SHA512") return SHA512.HashDataAsync(stream, hash, token);

        return ValueTask.FromException<int>(new ArgumentException($"Alg '{name}' not found", nameof(name)));
    }

#else

    public async ValueTask<int> HashAsync(Stream stream, Memory<byte> hash, string? name, CancellationToken token = default)
    {
        name ??= _hashNameDefault;

        if (name == "XXH32") return await XXH32.HashAsync(stream, hash, token);
        if (name == "XXH64") return await XXH64.HashAsync(stream, hash, token);
        if (name == "CRC32") return await CRC32.HashAsync(stream, hash, token);

        if (stream == null) throw new ArgumentNullException(nameof(stream));

        if (!stream.CanRead) throw new ArgumentException("Stream not readable", nameof(stream));

        using var alg = GetAlg(name);

        if (hash.Length < alg.Info.SizeInBytes) throw new ArgumentException("Destination too small", nameof(hash));

        await alg.AppendAsync(stream, token).ConfigureAwait(false);

        return alg.TryGetHash(hash.Span);
    }

#endif

    public ValueTask<uint> Hash32Async(Stream stream, string? name, CancellationToken token = default)
    {
        if (name == null || name == "XXH32") return XXH32.Hash32Async(stream, token);
        if (name == "CRC32") return CRC32.Hash32Async(stream, token);

        throw new ArgumentException($"Alg32 '{name}' not found", nameof(name));
    }

    public ValueTask<ulong> Hash64Async(Stream stream, string? name, CancellationToken token = default)
    {
        if (name == null || name == "XXH64") return XXH64.Hash64Async(stream, token);

        throw new ArgumentException($"Alg64 '{name}' not found", nameof(name));
    }
}