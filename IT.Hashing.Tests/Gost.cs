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

        using var nativeAlg = HashAlgorithms.CreateNativeGost3411_94();
        using var nativeAlgFirst = HashAlgorithms.CreateNativeGost3411_94(resetable: false);
        using var gostNative = new Gost_R3411_94_HashAlgorithm();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            if (i > 0)
            {
                nativeAlg.Reset();
            }

            nativeAlg.Append(bytes);

            var hash = GetHash(nativeAlg);

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411", bytes);

            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);

            if (i == 0)
            {
                nativeAlgFirst.Append(bytes);
                Assert.That(hash.SequenceEqual(GetHash(nativeAlgFirst)), Is.True);
            }
        }
    }

    [Test]
    public void Gost512()
    {
        var bytes = new byte[1024];

        using var nativeAlg = HashAlgorithms.CreateNativeGost3411_2012_512();
        using var nativeAlgFirst = HashAlgorithms.CreateNativeGost3411_2012_512(resetable: false);
        using var gostNative = new Gost_R3411_2012_512_HashAlgorithm();
        var gostManaged = new Gost3411_2012_512();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            if (i > 0)
            {
                nativeAlg.Reset();
                gostManaged.Reset();
            }

            nativeAlg.Append(bytes);
            gostManaged.Append(bytes);

            var hash = GetHash(nativeAlg);

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_512", bytes);

            var hash3 = GetHash(gostManaged);

            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);
            Assert.That(hash.SequenceEqual(hash3), Is.True);

            if (i == 0)
            {
                nativeAlgFirst.Append(bytes);
                Assert.That(hash.SequenceEqual(GetHash(nativeAlgFirst)), Is.True);
            }
        }
    }

    [Test]
    public void Gost256()
    {
        var bytes = new byte[1024];

        using var nativeAlg = HashAlgorithms.CreateNativeGost3411_2012_256();
        using var nativeAlgFirst = HashAlgorithms.CreateNativeGost3411_2012_256(resetable: false);
        using var gostNative = new Gost_R3411_2012_256_HashAlgorithm();
        var gostManaged = new Gost3411_2012_256();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            if (i > 0)
            {
                nativeAlg.Reset();
                gostManaged.Reset();
            }

            nativeAlg.Append(bytes);
            gostManaged.Append(bytes);

            var hash = GetHash(nativeAlg);

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_256", bytes);

            var hash3 = GetHash(gostManaged);

            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);
            Assert.That(hash.SequenceEqual(hash3), Is.True);

            if (i == 0)
            {
                nativeAlgFirst.Append(bytes);
                Assert.That(hash.SequenceEqual(GetHash(nativeAlgFirst)), Is.True);
            }
        }
    }

    private static byte[] GetHash(IHashAlgorithm alg)
    {
        var hash = new byte[alg.Size];
        alg.TryGetHash(hash, out _);

        return hash;
    }
}