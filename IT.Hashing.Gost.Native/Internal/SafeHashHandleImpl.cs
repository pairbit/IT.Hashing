using Microsoft.Win32.SafeHandles;
using System;
using System.Security;

namespace IT.Hashing.Gost.Native.Internal;

/// <summary>
/// Дескриптор функции хэширования криптографического провайдера.
/// </summary>
[SecurityCritical]
internal class SafeHashHandleImpl : SafeHandleZeroOrMinusOneIsInvalid
{
    public static SafeHashHandleImpl InvalidHandle => new SafeHashHandleImpl(IntPtr.Zero);


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
}