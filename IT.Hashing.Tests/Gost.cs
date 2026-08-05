using IT.Hashing.Gost;
using IT.Hashing.Gost.Native;
using Org.BouncyCastle.Security;

namespace IT.Hashing.Tests;

public class Gost
{
    private static readonly Random _random = new();

    [Test]
    public void Gost94()
    {
        var bytes = new byte[1024];

        using var nativeAlg = HashAlgorithms.CreateGost3411_94();
        using var gostNative = new Gost_R3411_94_HashAlgorithm();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            var hash = CalcAlgorithm(nativeAlg, bytes);

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411", bytes);

            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);
        }
    }

    [Test]
    public void Gost512()
    {
        var bytes = new byte[1024];

        using var nativeAlg = HashAlgorithms.CreateGost3411_2012_512();
        using var gostNative = new Gost_R3411_2012_512_HashAlgorithm();
        var gostManaged = new Gost3411_2012_512();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            var hash = CalcAlgorithm(nativeAlg, bytes);

            var hash1 = gostNative.ComputeHash(bytes);
            
            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_512", bytes);

            var hash3 = CalcAlgorithm(gostManaged, bytes);
            
            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);
            Assert.That(hash.SequenceEqual(hash3), Is.True);
        }
    }

    [Test]
    public void Gost256()
    {
        var bytes = new byte[1024];

        using var nativeAlg = HashAlgorithms.CreateGost3411_2012_256();
        using var gostNative = new Gost_R3411_2012_256_HashAlgorithm();
        var gostManaged = new Gost3411_2012_256();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            var hash = CalcAlgorithm(nativeAlg, bytes);

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_256", bytes);

            var hash3 = CalcAlgorithm(gostManaged, bytes);

            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);
            Assert.That(hash.SequenceEqual(hash3), Is.True);
        }
    }

    private static byte[] CalcAlgorithm(IHashAlgorithm alg, ReadOnlySpan<byte> data)
    {
        alg.Append(data);

        var hash = new byte[alg.Size];

        alg.TryGetHash(hash, out _);
        alg.Reset();

        return hash;
    }
}