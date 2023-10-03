using System;
using System.Runtime.InteropServices;

namespace IT.Hashing.Gost.Native.Internal;

internal class WindowsApi : INativeApi
{
    public WindowsApi()
    {
        ExceptionUtility.Log("Detect WindowsApi");
    }

    public bool CryptAcquireContext([In, Out] ref SafeProvHandleImpl hProv, [In] string pszContainer, [In] string pszProvider, [In] uint dwProvType, [In] uint dwFlags)
    {
        return WindowsNativeApi.CryptAcquireContext(ref hProv, pszContainer, pszProvider, dwProvType, dwFlags);
    }

    public bool CryptContextAddRef([In] IntPtr hProv, [In] byte[] pdwReserved, [In] uint dwFlags)
    {
        return WindowsNativeApi.CryptContextAddRef(hProv, pdwReserved, dwFlags);
    }

    public bool CryptCreateHash([In] SafeProvHandleImpl hProv, [In] uint Algid, [In] SafeKeyHandleImpl hKey, [In] uint dwFlags, [In, Out] ref SafeHashHandleImpl phHash)
    {
        return WindowsNativeApi.CryptCreateHash(hProv, Algid, hKey, dwFlags, ref phHash);
    }

    public bool CryptDestroyHash(IntPtr pHashCtx)
    {
        return WindowsNativeApi.CryptDestroyHash(pHashCtx);
    }

    public bool CryptDestroyKey(IntPtr pKeyCtx)
    {
        return WindowsNativeApi.CryptDestroyKey(pKeyCtx);
    }

    public bool CryptGetHashParam([In] SafeHashHandleImpl hHash, [In] uint dwParam, [In, Out] byte[] pbData, ref uint pdwDataLen, [In] uint dwFlags)
    {
        return WindowsNativeApi.CryptGetHashParam(hHash, dwParam, pbData, ref pdwDataLen, dwFlags);
    }

    public bool CryptHashData([In] SafeHashHandleImpl hHash, [In, Out] byte[] pbData, [In] uint dwDataLen, [In] uint dwFlags)
    {
        return WindowsNativeApi.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
    }

    public unsafe bool CryptHashData([In] SafeHashHandleImpl hHash, byte* pbData, [In] uint dwDataLen, [In] uint dwFlags)
    {
        return WindowsNativeApi.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
    }

    public bool CryptReleaseContext(IntPtr hCryptProv, uint dwFlags)
    {
        return WindowsNativeApi.CryptReleaseContext(hCryptProv, dwFlags);
    }

    public bool CryptSetProvParam([In] SafeProvHandleImpl hProv, [In] uint dwParam, [In] IntPtr pbData, [In] uint dwFlags)
    {
        return WindowsNativeApi.CryptSetProvParam(hProv, dwParam, pbData, dwFlags);
    }

    public bool CryptSetProvParam2(IntPtr hCryptProv, [In] uint dwParam, [In] byte[] pbData, [In] uint dwFlags)
    {
        return WindowsNativeApi.CryptSetProvParam2(hCryptProv, dwParam, pbData, dwFlags);
    }
}
