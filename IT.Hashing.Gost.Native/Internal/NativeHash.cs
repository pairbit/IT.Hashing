using Microsoft.Win32.SafeHandles;
using System;
using System.Security;

namespace IT.Hashing.Gost.Native.Internal;

/// <summary>
/// Дескриптор функции хэширования криптографического провайдера.
/// </summary>
[SecurityCritical]
internal class NativeHash : SafeHandleZeroOrMinusOneIsInvalid
{
    public static NativeHash InvalidHandle => new NativeHash(IntPtr.Zero);

    public NativeHash() : base(true)
    {
    }

    public NativeHash(IntPtr handle) : base(true)
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