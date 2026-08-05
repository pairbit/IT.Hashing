using IT.Hashing.Gost;
using IT.Hashing.Gost.Native;
using Org.BouncyCastle.Security;

namespace IT.Hashing.Tests;

public class Gost
{
    private static readonly Random _random = new();

    [Test]
    public void Gost512()
    {
        var bytes = new byte[1024];

        using var gostNative = new Gost_R3411_2012_512_HashAlgorithm();
        var gostManaged = new Gost3411_2012_512Digest();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            var hash = CalculateNative(gostNative, bytes);

            var hash1 = gostNative.ComputeHash(bytes);
            
            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_512", bytes);

            var hash3 = CalculateDigest(gostManaged, bytes);
            
            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);
            Assert.That(hash.SequenceEqual(hash3), Is.True);
        }
    }

    [Test]
    public void Gost256()
    {
        var bytes = new byte[1024];

        using var gostNative = new Gost_R3411_2012_256_HashAlgorithm();
        var gostManaged = new Gost3411_2012_256Digest();

        for (int i = 0; i < 100; i++)
        {
            _random.NextBytes(bytes);

            var hash = CalculateNative(gostNative, bytes);

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_256", bytes);

            var hash3 = CalculateDigest(gostManaged, bytes);

            Assert.That(hash.SequenceEqual(hash1), Is.True);
            Assert.That(hash.SequenceEqual(hash2), Is.True);
            Assert.That(hash.SequenceEqual(hash3), Is.True);
        }
    }

    //GOST3411, GOST3411_2012_512
    private static byte[] CalculateDigest(Gost3411_2012Digest digest, byte[] input)
    {
        digest.BlockUpdate(input, 0, input.Length);

        byte[] b = new byte[3 + digest.GetDigestSize()];

        digest.DoFinal(b, 3);

        return b.AsSpan(3).ToArray();
    }

    private static byte[] CalculateNative(Gost_R3411_HashAlgorithm hashAlg, ReadOnlySpan<byte> data)
    {
        hashAlg.HashData(data);

        byte[] hash = new byte[256];

        var written = hashAlg.HashFinal(hash);

        hashAlg.Initialize();

        return hash.AsSpan(0, written).ToArray();
    }
}