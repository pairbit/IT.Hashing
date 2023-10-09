namespace IT.Hashing.Tests;

public class HasherTest
{
    private static readonly Random _random = new();
    private static readonly Hasher _hasherDefault = new();

    private readonly IHasher _hasher = _hasherDefault;
    private readonly IHasher32 _hasher32 = _hasherDefault;
    private readonly IHasher64 _hasher64 = _hasherDefault;

    [Test]
    public void Test()
    {
        var bytes = new byte[1024];
        _random.NextBytes(bytes);

        var infos = _hasher.Info;

        for (int i = 0; i < infos.Count; i++)
        {
            var info = infos[i];
            var name = info.Name;

            var hash = new byte[info.SizeInBytes];

            Assert.That(_hasher.TryHash(bytes, hash, name), Is.EqualTo(info.SizeInBytes));

            Assert.That(hash.SequenceEqual(_hasher.Hash(bytes, name)), Is.True);

            Assert.That(hash.SequenceEqual(_hasher.Hash(bytes, 0, bytes.Length, name)), Is.True);

            var alg = _hasher.GetAlg(name);

            alg.Append(bytes);

            var hash3 = alg.GetHash();

            Assert.That(hash.SequenceEqual(hash3), Is.True);
        }
    }
}