using IT.Hashing.Gost.Internal;
using System;
using System.Runtime.CompilerServices;

namespace IT.Hashing.Gost;

public class Gost3411_2012_256 : Gost3411_2012_512
{
    private const int HalfBlockSizeWords = BlockSizeWords / 2;

    public override int Size => 32;

    /// <exception cref="ObjectDisposedException">Thrown when the instance has been disposed.</exception>
    [MethodImpl(MethodImplOptionsEx.OptimizedLoop)]
    public override bool TryGetHash(Span<byte> destination, out int length)
    {
        length = 32;
        if (destination.Length < length)
            return false;

        BinarySpans.WriteUInt64LittleEndian(HashFinal().AsSpan(HalfBlockSizeWords, HalfBlockSizeWords), destination);

        return true;
    }

    public override void Reset()
    {
        // IV is 0x00 for 512-bit, 0x01 for 256-bit (RFC 6986 Section 6.1)
        Reset(0x0101010101010101UL);
    }
}