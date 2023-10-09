using System;
using System.Runtime.CompilerServices;

namespace IT.Hashing;

//https://github.com/MiloszKrajewski/K4os.Hash.xxHash
public abstract class XXH : IHashAlg
{
    public abstract HashInfo Info { get; }

    protected XXH()
    {

    }

    public abstract void Append(ReadOnlySpan<byte> bytes);

    public abstract void Append(byte[] bytes, int offset, int count);

    public abstract void Reset();

    public abstract int TryGetHash(Span<byte> hash);

    public abstract byte[] GetHash();

    public void Dispose() { }

#if NETSTANDARD2_0

    public void Append(byte[] bytes) => Append(bytes.AsSpan());

    public void Append(System.IO.Stream stream) => Internal.StreamHasher.Append(stream, Append);

    public System.Threading.Tasks.Task AppendAsync(System.IO.Stream stream, System.Threading.CancellationToken token = default) => Internal.StreamHasher.AppendAsync(stream, Append, token);

#endif

    #region static

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe static uint XXH_read32(void* p) => *(uint*)p;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal unsafe static ulong XXH_read64(void* p) => *(ulong*)p;

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    internal unsafe static void XXH_zero(void* target, int length)
    {
        var targetP = (byte*)target;

        while (length >= sizeof(ulong))
        {
            *(ulong*)targetP = 0;
            targetP += sizeof(ulong);
            length -= sizeof(ulong);
        }

        if (length >= sizeof(uint))
        {
            *(uint*)targetP = 0;
            targetP += sizeof(uint);
            length -= sizeof(uint);
        }

        if (length >= sizeof(ushort))
        {
            *(ushort*)targetP = 0;
            targetP += sizeof(ushort);
            length -= sizeof(ushort);
        }

        if (length > 0)
        {
            *targetP = 0;
            // targetP++;
            // length--;
        }
    }

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    internal unsafe static void XXH_copy(void* target, void* source, int length)
    {
        var sourceP = (byte*)source;
        var targetP = (byte*)target;

        while (length >= sizeof(ulong))
        {
            *(ulong*)targetP = *(ulong*)sourceP;
            targetP += sizeof(ulong);
            sourceP += sizeof(ulong);
            length -= sizeof(ulong);
        }

        if (length >= sizeof(uint))
        {
            *(uint*)targetP = *(uint*)sourceP;
            targetP += sizeof(uint);
            sourceP += sizeof(uint);
            length -= sizeof(uint);
        }

        if (length >= sizeof(ushort))
        {
            *(ushort*)targetP = *(ushort*)sourceP;
            targetP += sizeof(ushort);
            sourceP += sizeof(ushort);
            length -= sizeof(ushort);
        }

        if (length > 0)
        {
            *targetP = *sourceP;
            // targetP++;
            // sourceP++;
            // length--;
        }
    }

    internal static void Validate(byte[] bytes, int offset, int length)
    {
        if (bytes == null || offset < 0 || length < 0 || offset + length > bytes.Length)
            throw new ArgumentException("Invalid buffer boundaries");
    }

    #endregion static
}