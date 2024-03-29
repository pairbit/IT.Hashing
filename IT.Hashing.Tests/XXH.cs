using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace IT.Hashing.Tests;

public class XXH
{
    private static readonly Random _random = new();

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Check32()
    {
        var bytes = new byte[1024];
        _random.NextBytes(bytes);
        Check32(bytes);

        for (int i = 16; i >= 0; i--)
        {
            bytes = new byte[i];
            _random.NextBytes(bytes);
            Check32(bytes);
        }
    }

    [Test]
    public void Check64()
    {
        var bytes = new byte[1024];
        _random.NextBytes(bytes);
        Check64(bytes);

        for (int i = 16; i >= 0; i--)
        {
            bytes = new byte[i];
            _random.NextBytes(bytes);
            Check64(bytes);
        }
    }

    private void Check32(byte[] bytes)
    {
        var bhash = System.IO.Hashing.XxHash32.Hash(bytes);
        var ihash = BinaryPrimitives.ReadUInt32BigEndian(bhash);

        var ihash2 = XXH32.Hash32(bytes);
        var bhash2 = new byte[4];

        BinaryPrimitives.WriteUInt32BigEndian(bhash2, ihash2);

        Assert.That(ihash, Is.EqualTo(ihash2));

        Assert.That(bhash.SequenceEqual(bhash2), Is.True);
    }

    private void Check64(byte[] bytes)
    {
        var bhash = System.IO.Hashing.XxHash64.Hash(bytes);
        var ihash = BinaryPrimitives.ReadUInt64BigEndian(bhash);

        var ihash2 = XXH64.Hash64(bytes);
        var bhash2 = new byte[8];

        BinaryPrimitives.WriteUInt64BigEndian(bhash2, ihash2);

        Assert.That(ihash, Is.EqualTo(ihash2));

        Assert.That(bhash.SequenceEqual(bhash2), Is.True);
    }
}