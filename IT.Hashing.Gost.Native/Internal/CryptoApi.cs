using System;
using System.Security;

namespace IT.Hashing.Gost.Native.Internal;

/// <summary>
/// Функции для работы с Microsoft CryptoAPI.
/// </summary>
[SecurityCritical]
internal static class CryptoApi
{
    private readonly static INativeApi Api = GetNativeApi();

    public static bool IsLinux(PlatformID platformID)
    {
        int p = (int)platformID;
        return p == 4 || p == 6 || p == 128;
    }

    private static INativeApi GetNativeApi()
    {
        var platform = Environment.OSVersion.Platform;

        ExceptionUtility.Log($"GetNativeApi platform = '{platform}'");

        if (IsLinux(platform)) return new LinuxApi(ProviderType.CryptoPro);
        if (platform == PlatformID.Win32NT) return new WindowsApi();

        throw new NotSupportedException(string.Format("Platform '{0}' not supported", platform));
    }

    public static bool CryptAcquireContext(ref SafeProvHandleImpl hProv, string pszContainer, string pszProvider, uint dwProvType, uint dwFlags)
    {
        ExceptionUtility.Log($"CryptAcquireContext (?, '{pszContainer}', '{pszProvider}', '{dwProvType}', '{dwFlags}')");
        return Api.CryptAcquireContext(ref hProv, pszContainer, pszProvider, dwProvType, dwFlags);
    }

    public static bool CryptContextAddRef(IntPtr hProv, byte[]? pdwReserved, uint dwFlags)
    {
        return Api.CryptContextAddRef(hProv, pdwReserved, dwFlags);
    }

    public static bool CryptCreateHash(SafeProvHandleImpl hProv, uint Algid, SafeKeyHandleImpl hKey, uint dwFlags, ref SafeHashHandleImpl phHash)
    {
        return Api.CryptCreateHash(hProv, Algid, hKey, dwFlags, ref phHash);
    }

    public static bool CryptDestroyHash(IntPtr pHashCtx)
    {
        return Api.CryptDestroyHash(pHashCtx);
    }

    public static bool CryptDestroyKey(IntPtr pKeyCtx)
    {
        return Api.CryptDestroyKey(pKeyCtx);
    }

    public static bool CryptGetHashParam(SafeHashHandleImpl hHash, uint dwParam, byte[]? pbData, ref uint pdwDataLen, uint dwFlags)
    {
        return Api.CryptGetHashParam(hHash, dwParam, pbData, ref pdwDataLen, dwFlags);
    }

    public static bool CryptReleaseContext(IntPtr hCryptProv, uint dwFlags)
    {
        return Api.CryptReleaseContext(hCryptProv, dwFlags);
    }

    public static bool CryptSetProvParam(SafeProvHandleImpl hProv, uint dwParam, IntPtr pbData, uint dwFlags)
    {
        return Api.CryptSetProvParam(hProv, dwParam, pbData, dwFlags);
    }

    public static bool CryptSetProvParam2(IntPtr hCryptProv, uint dwParam, byte[]? pbData, uint dwFlags)
    {
        return Api.CryptSetProvParam2(hCryptProv, dwParam, pbData, dwFlags);
    }

    public static bool CryptHashData(SafeHashHandleImpl hHash, byte[] pbData, uint dwDataLen, uint dwFlags)
    {
        return Api.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
    }

    public static unsafe bool CryptHashData(SafeHashHandleImpl hHash, byte* pbData, uint dwDataLen, uint dwFlags)
    {
        return Api.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
    }
}