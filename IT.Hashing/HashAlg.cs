using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public class HashAlg : IHashAlg
{
    public HashInfo Info => throw new NotImplementedException();

    public void Append(ReadOnlySpan<byte> bytes)
    {
        throw new NotImplementedException();
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