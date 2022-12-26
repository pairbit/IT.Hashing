using System.Buffers.Binary;

namespace IT.Hashing.Tests;

public class XXH
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Check32()
    {
        var bytes = new byte[1024];

        Random.Shared.NextBytes(bytes);

        var bhash = System.IO.Hashing.XxHash32.Hash(bytes);
        var ihash = BinaryPrimitives.ReadUInt32BigEndian(bhash);

        var ihash2 = XXH32.DigestOf(bytes);
        var bhash2 = new byte[4];

        BinaryPrimitives.WriteUInt32BigEndian(bhash2, ihash2);

        Assert.That(ihash, Is.EqualTo(ihash2));

        Assert.That(bhash.SequenceEqual(bhash2), Is.True);
    }

    [Test]
    public void Check64()
    {
        var bytes = new byte[1024];

        Random.Shared.NextBytes(bytes);

        var bhash = System.IO.Hashing.XxHash64.Hash(bytes);
        var ihash = BinaryPrimitives.ReadUInt64BigEndian(bhash);

        var ihash2 = XXH64.DigestOf(bytes);
        var bhash2 = new byte[8];

        BinaryPrimitives.WriteUInt64BigEndian(bhash2, ihash2);

        Assert.That(ihash, Is.EqualTo(ihash2));

        Assert.That(bhash.SequenceEqual(bhash2), Is.True);
    }
}