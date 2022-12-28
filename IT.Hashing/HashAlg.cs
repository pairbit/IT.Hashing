using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public class HashAlg : IHashAlg
{
    private HashAlgorithm _algorithm;

    public HashInfo Info => throw new NotImplementedException();

    public HashAlg(HashAlgorithm algorithm)
    {
        _algorithm = algorithm;
    }

    public void Append(ReadOnlySpan<byte> bytes)
    {

    }

    public void Append(Stream stream)
    {
        throw new NotImplementedException();
    }

    public Task AppendAsync(Stream stream, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public bool TryHash(Span<byte> hash)
    {
        throw new NotImplementedException();
    }
}