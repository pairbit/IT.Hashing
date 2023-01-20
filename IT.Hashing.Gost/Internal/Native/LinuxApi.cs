using GostCryptography.Base;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GostCryptography.Native
{
    class LinuxApi : INativeApi
    {
        private readonly ProviderType _providerType;

        public LinuxApi(ProviderType type)
        {
            ExceptionUtility.Log($"Detect LinuxApi, IsCryptoPro = '{type.IsCryptoPro()}'");
            _providerType = type;
        }

        public bool CryptAcquireContext([In, Out] ref SafeProvHandleImpl hProv, [In] string pszContainer,
            [In] string pszProvider, [In] uint dwProvType, [In] uint dwFlags)
        {
            bool result;
            try
            {
                //This string depends on encoding of system
                var containerNamePtr = MarshalString(pszContainer);
                var providerNamePtr = MarshalString(pszProvider);
                result = _providerType.IsCryptoPro()
                    ? LinuxCryptoProNativeApi.CryptAcquireContext(ref hProv, containerNamePtr, providerNamePtr, dwProvType, dwFlags)
                    : LinuxVipNetNativeApi.CryptAcquireContext(ref hProv, containerNamePtr, providerNamePtr, dwProvType, dwFlags);

                Marshal.FreeHGlobal(containerNamePtr);
                Marshal.FreeHGlobal(providerNamePtr);
            }
            catch (Exception ex)
            {
                ExceptionUtility.Log($"CryptAcquireContext Exception", ex);
                result = false;
            }

            return result;
        }

        public bool CryptContextAddRef([In] IntPtr hProv, [In] byte[] pdwReserved, [In] uint dwFlags)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptContextAddRef(hProv, pdwReserved, dwFlags);
            else
                return LinuxVipNetNativeApi.CryptContextAddRef(hProv, pdwReserved, dwFlags);
        }

        public bool CryptCreateHash([In] SafeProvHandleImpl hProv, [In] uint Algid, [In] SafeKeyHandleImpl hKey, [In] uint dwFlags, [In, Out] ref SafeHashHandleImpl phHash)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptCreateHash(hProv, Algid, hKey, dwFlags, ref phHash);
            else
                return LinuxVipNetNativeApi.CryptCreateHash(hProv, Algid, hKey, dwFlags, ref phHash);
        }

        public bool CryptDestroyHash(IntPtr pHashCtx)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptDestroyHash(pHashCtx);
            else
                return LinuxVipNetNativeApi.CryptDestroyHash(pHashCtx);
        }

        public bool CryptDestroyKey(IntPtr pKeyCtx)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptDestroyKey(pKeyCtx);
            else
                return LinuxVipNetNativeApi.CryptDestroyKey(pKeyCtx);
        }

        public bool CryptGetHashParam([In] SafeHashHandleImpl hHash, [In] uint dwParam, [In, Out] byte[] pbData, ref uint pdwDataLen, [In] uint dwFlags)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptGetHashParam(hHash, dwParam, pbData, ref pdwDataLen, dwFlags);
            else
                return LinuxVipNetNativeApi.CryptGetHashParam(hHash, dwParam, pbData, ref pdwDataLen, dwFlags);
        }

        public bool CryptHashData([In] SafeHashHandleImpl hHash, [In, Out] byte[] pbData, [In] uint dwDataLen, [In] uint dwFlags)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
            else
                return LinuxVipNetNativeApi.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
        }

        public unsafe bool CryptHashData([In] SafeHashHandleImpl hHash, byte* pbData, [In] uint dwDataLen, [In] uint dwFlags)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
            else
                return LinuxVipNetNativeApi.CryptHashData(hHash, pbData, dwDataLen, dwFlags);
        }

        public bool CryptReleaseContext(IntPtr hCryptProv, uint dwFlags)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptReleaseContext(hCryptProv, dwFlags);
            else
                return LinuxVipNetNativeApi.CryptReleaseContext(hCryptProv, dwFlags);
        }

        public bool CryptSetProvParam([In] SafeProvHandleImpl hProv, [In] uint dwParam, [In] IntPtr pbData, [In] uint dwFlags)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptSetProvParam(hProv, dwParam, pbData, dwFlags);
            else
                return LinuxVipNetNativeApi.CryptSetProvParam(hProv, dwParam, pbData, dwFlags);
        }

        public bool CryptSetProvParam2(IntPtr hCryptProv, [In] uint dwParam, [In] byte[] pbData, [In] uint dwFlags)
        {
            if (_providerType.IsCryptoPro())
                return LinuxCryptoProNativeApi.CryptSetProvParam2(hCryptProv, dwParam, pbData, dwFlags);
            else
                return LinuxVipNetNativeApi.CryptSetProvParam2(hCryptProv, dwParam, pbData, dwFlags);
        }

        private IntPtr MarshalString(string str)
        {
            if (str == null) return IntPtr.Zero;
            str += '\0'; //add end of string
            var buffer = Encoding.UTF32.GetBytes(str);
            var result = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, result, buffer.Length);
            return result;
        }
    }
}
