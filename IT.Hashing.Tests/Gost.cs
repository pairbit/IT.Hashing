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

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_512", bytes);

            var hash3 = CalculateDigest(gostManaged, bytes);

            Assert.That(hash1.SequenceEqual(hash2), Is.True);
            Assert.That(hash1.SequenceEqual(hash3), Is.True);
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

            var hash1 = gostNative.ComputeHash(bytes);

            var hash2 = DigestUtilities.CalculateDigest("GOST3411_2012_256", bytes);

            var hash3 = CalculateDigest(gostManaged, bytes);

            Assert.That(hash1.SequenceEqual(hash2), Is.True);
            Assert.That(hash1.SequenceEqual(hash3), Is.True);
        }
    }

    //GOST3411, GOST3411_2012_512
    private static byte[] CalculateDigest(Gost3411_2012Digest digest, byte[] input)
    {
        digest.BlockUpdate(input, 0, input.Length);

        byte[] b = new byte[digest.GetDigestSize()];

        digest.DoFinal(b, 0);

        return b;
    }
}