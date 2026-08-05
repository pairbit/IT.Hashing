using Microsoft.Win32.SafeHandles;
using System;
using System.Security;

namespace IT.Hashing.Gost.Native.Internal;

/// <summary>
/// Дескриптор функции хэширования криптографического провайдера.
/// </summary>
[SecurityCritical]
internal class SafeHashHandleImpl : SafeHandleZeroOrMinusOneIsInvalid, IHashAlgorithm
{
    public static SafeHashHandleImpl InvalidHandle => new SafeHashHandleImpl(IntPtr.Zero);

    public int Size => CryptoApiHelper.GetEndHashDataLength(this);

    public SafeHashHandleImpl() : base(true)
    {
    }

    public SafeHashHandleImpl(IntPtr handle) : base(true)
    {
        SetHandle(handle);
    }

    [SecurityCritical]
    protected override bool ReleaseHandle()
    {
        CryptoApi.CryptDestroyHash(handle);
        return true;
    }

    [SecurityCritical]
    void IDisposable.Dispose()
    {
        this.TryDispose();
    }

    [SecurityCritical]
    public void Append(byte value)
    {
        Span<byte> bytes = stackalloc byte[1];
        bytes[0] = value;

        CryptoApiHelper.HashData(this, bytes);
    }

    [SecurityCritical]
    public void Append(ReadOnlySpan<byte> bytes)
    {
        CryptoApiHelper.HashData(this, bytes);
    }

    [SecurityCritical]
    public void Append(byte[] array, int start, int length)
    {
        CryptoApiHelper.HashData(this, array, start, length);
    }

    [SecurityCritical]
    public bool TryGetHash(Span<byte> hash, out int written)
    {
        return CryptoApiHelper.TryGetEndHashData(this, hash, out written);
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}