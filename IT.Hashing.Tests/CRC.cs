using System.Buffers.Binary;

namespace IT.Hashing.Tests;

public class CRC
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

    private void Check32(byte[] bytes)
    {
        var bhash = System.IO.Hashing.Crc32.Hash(bytes);
        var ihash = BinaryPrimitives.ReadUInt32LittleEndian(bhash);

        var ihash2 = CRC32.Hash32(bytes);
        var bhash2 = new byte[4];

        BinaryPrimitives.WriteUInt32LittleEndian(bhash2, ihash2);

        Assert.That(ihash, Is.EqualTo(ihash2));

        Assert.That(bhash.SequenceEqual(bhash2), Is.True);

        var ihash3 = Force.Crc32.Crc32Algorithm.Compute(bytes);

        Assert.That(ihash, Is.EqualTo(ihash3));
    }
}