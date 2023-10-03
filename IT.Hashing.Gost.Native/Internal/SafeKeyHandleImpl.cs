using Microsoft.Win32.SafeHandles;
using System;
using System.Security;

namespace IT.Hashing.Gost.Native.Internal;

/// <summary>
/// Дескриптор ключа криптографического провайдера.
/// </summary>
[SecurityCritical]
internal sealed class SafeKeyHandleImpl : SafeHandleZeroOrMinusOneIsInvalid
{
    public SafeKeyHandleImpl()
        : base(true)
    {
    }

    public SafeKeyHandleImpl(IntPtr handle)
        : base(true)
    {
        SetHandle(handle);
    }

    public static SafeKeyHandleImpl InvalidHandle
    {
        get { return new SafeKeyHandleImpl(IntPtr.Zero); }
    }

    [SecurityCritical]
    protected override bool ReleaseHandle()
    {
        CryptoApi.CryptDestroyKey(handle);
        return true;
    }
}