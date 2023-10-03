using System;
using System.Runtime.InteropServices;

namespace IT.Hashing.Gost.Native.Internal;

internal interface INativeApi
{
    bool CryptAcquireContext([In][Out] ref SafeProvHandleImpl hProv, [In] string pszContainer, [In] string pszProvider, [In] uint dwProvType, [In] uint dwFlags);

    bool CryptReleaseContext(IntPtr hCryptProv, uint dwFlags);

    bool CryptContextAddRef([In] IntPtr hProv, [In] byte[]? pdwReserved, [In] uint dwFlags);

    bool CryptSetProvParam([In] SafeProvHandleImpl hProv, [In] uint dwParam, [In] IntPtr pbData, [In] uint dwFlags);

    bool CryptSetProvParam2(IntPtr hCryptProv, [In] uint dwParam, [In] byte[]? pbData, [In] uint dwFlags);

    bool CryptCreateHash([In] SafeProvHandleImpl hProv, [In] uint Algid, [In] SafeKeyHandleImpl hKey, [In] uint dwFlags, [In][Out] ref SafeHashHandleImpl phHash);

    bool CryptDestroyHash(IntPtr pHashCtx);

    bool CryptGetHashParam([In] SafeHashHandleImpl hHash, [In] uint dwParam, [In][Out] byte[]? pbData, ref uint pdwDataLen, [In] uint dwFlags);

    bool CryptHashData([In] SafeHashHandleImpl hHash, [In][Out] byte[] pbData, [In] uint dwDataLen, [In] uint dwFlags);

    unsafe bool CryptHashData([In] SafeHashHandleImpl hHash, byte* pbData, [In] uint dwDataLen, [In] uint dwFlags);

    bool CryptDestroyKey(IntPtr pKeyCtx);
}
