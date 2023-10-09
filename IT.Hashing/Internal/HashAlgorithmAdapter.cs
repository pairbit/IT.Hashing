using System;
using System.Security.Cryptography;

namespace IT.Hashing.Internal;

internal class HashAlgorithmAdapter : HashAlgorithm
{
    private readonly IHashAlg _alg;

    public HashAlgorithmAdapter(IHashAlg alg)
    {
        _alg = alg ?? throw new ArgumentNullException(nameof(alg));
        HashSizeValue = alg.Info.SizeInBytes * 8;
    }

    public override void Initialize() => _alg.Reset();

    protected override void HashCore(byte[] array, int ibStart, int cbSize) => _alg.Append(array, ibStart, cbSize);

    protected override byte[] HashFinal() => _alg.GetHash();

#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER

    protected override void HashCore(ReadOnlySpan<byte> source) => _alg.Append(source);

    protected override bool TryHashFinal(Span<byte> destination, out int bytesWritten)
    {
        bytesWritten = _alg.TryGetHash(destination);
        return bytesWritten > 0;
    }

#endif
}