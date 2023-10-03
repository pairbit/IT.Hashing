using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Threading;

namespace IT.Hashing.Gost.Native.Internal;

/// <summary>
/// Вспомогательные методы для работы с Microsoft CryptoAPI.
/// </summary>
[SecurityCritical]
internal static class CryptoApiHelper
{
    /// <summary>
    /// Возвращает <see langword="true"/>, если заданный провайдер установлен.
    /// </summary>
    [SecurityCritical]
    public static bool IsInstalled(ProviderType providerType)
    {
        try
        {
            var providerHandle = GetProviderHandle(providerType);
            return !providerHandle.IsInvalid;
        }
        catch (Exception ex)
        {
            ExceptionUtility.Log($"Check Installed '{providerType}'", ex);
            return false;
        }
    }

    /// <summary>
    /// Возвращает доступный провайдер для ключей ГОСТ Р 34.10-2001.
    /// </summary>
    /// <exception cref="CryptographicException">Провайдер не установлен.</exception>
    [SecuritySafeCritical]
    public static ProviderType GetAvailableProviderType_2001()
    {
        if (IsInstalled(ProviderType.VipNet))
        {
            return ProviderType.VipNet;
        }

        if (IsInstalled(ProviderType.CryptoPro))
        {
            return ProviderType.CryptoPro;
        }

        throw ExceptionUtility.CryptographicException("GOST R 34.10-2001 Cryptographic Service Provider is not installed");
    }

    /// <summary>
    /// Возвращает доступный провайдер для ключей ГОСТ Р 34.10-2012/512.
    /// </summary>
    /// <exception cref="CryptographicException">Провайдер не установлен.</exception>
    [SecuritySafeCritical]
    public static ProviderType GetAvailableProviderType_2012_512()
    {
        if (IsInstalled(ProviderType.VipNet_2012_512))
        {
            return ProviderType.VipNet_2012_512;
        }

        if (IsInstalled(ProviderType.CryptoPro_2012_512))
        {
            return ProviderType.CryptoPro_2012_512;
        }

        throw ExceptionUtility.CryptographicException("GOST R 34.10-2012/512 Cryptographic Service Provider is not installed");
    }

    /// <summary>
    /// Возвращает доступный провайдер для ключей ГОСТ Р 34.10-2012/1024.
    /// </summary>
    /// <exception cref="CryptographicException">Провайдер не установлен.</exception>
    [SecuritySafeCritical]
    public static ProviderType GetAvailableProviderType_2012_1024()
    {
        if (IsInstalled(ProviderType.VipNet_2012_1024))
        {
            return ProviderType.VipNet_2012_1024;
        }

        if (IsInstalled(ProviderType.CryptoPro_2012_1024))
        {
            return ProviderType.CryptoPro_2012_1024;
        }

        throw ExceptionUtility.CryptographicException("GOST R 34.10-2012/1024 Cryptographic Service Provider is not installed");
    }


    #region Общие объекты

    private static readonly object ProviderHandleSync = new object();
    private static volatile Dictionary<ProviderType, SafeProvHandleImpl> _providerHandles = new Dictionary<ProviderType, SafeProvHandleImpl>();

    public static SafeProvHandleImpl GetProviderHandle(ProviderType providerType)
    {
        if (!_providerHandles.ContainsKey(providerType))
        {
            lock (ProviderHandleSync)
            {
                if (!_providerHandles.ContainsKey(providerType))
                {
                    var providerParams = new CspParameters(providerType.ToInt());
                    var providerHandle = AcquireProvider(providerParams);

                    Thread.MemoryBarrier();

                    _providerHandles.Add(providerType, providerHandle);
                }
            }
        }

        return _providerHandles[providerType];
    }

    #endregion


    #region Для работы с криптографическим провайдером

    public static SafeProvHandleImpl AcquireProvider(CspParameters providerParameters)
    {
        var providerHandle = SafeProvHandleImpl.InvalidHandle;

        var dwFlags = Constants.CRYPT_VERIFYCONTEXT;

        if ((providerParameters.Flags & CspProviderFlags.UseMachineKeyStore) != CspProviderFlags.NoFlags)
        {
            dwFlags |= Constants.CRYPT_MACHINE_KEYSET;
        }

        if (!CryptoApi.CryptAcquireContext(ref providerHandle, providerParameters.KeyContainerName, providerParameters.ProviderName, (uint)providerParameters.ProviderType, dwFlags))
        {
            throw CreateWin32Error();
        }

        return providerHandle;
    }

    #endregion


    #region Для работы с функцией хэширования криптографического провайдера

    public static SafeHashHandleImpl CreateHash_3411_94(SafeProvHandleImpl providerHandle)
    {
        return CreateHash_3411(providerHandle, Constants.CALG_GR3411);
    }

    public static SafeHashHandleImpl CreateHash_3411_2012_256(SafeProvHandleImpl providerHandle)
    {
        return CreateHash_3411(providerHandle, Constants.CALG_GR3411_2012_256);
    }

    public static SafeHashHandleImpl CreateHash_3411_2012_512(SafeProvHandleImpl providerHandle)
    {
        return CreateHash_3411(providerHandle, Constants.CALG_GR3411_2012_512);
    }

    private static SafeHashHandleImpl CreateHash_3411(SafeProvHandleImpl providerHandle, int hashAlgId)
    {
        var hashHandle = SafeHashHandleImpl.InvalidHandle;

        if (!CryptoApi.CryptCreateHash(providerHandle, (uint)hashAlgId, SafeKeyHandleImpl.InvalidHandle, 0, ref hashHandle))
        {
            throw CreateWin32Error();
        }

        return hashHandle;
    }

    public static unsafe void HashData(SafeHashHandleImpl hashHandle, byte[] data, int dataOffset, int dataLength)
    {
        if (data == null)
        {
            throw ExceptionUtility.ArgumentNull(nameof(data));
        }

        if (dataOffset < 0)
        {
            throw ExceptionUtility.ArgumentOutOfRange(nameof(dataOffset));
        }

        if (dataLength < 0)
        {
            throw ExceptionUtility.ArgumentOutOfRange(nameof(dataLength));
        }

        if (data.Length < dataOffset + dataLength)
        {
            throw ExceptionUtility.ArgumentOutOfRange(nameof(dataLength));
        }

        if (dataLength > 0)
        {
            fixed (byte* dataRef = data)
            {
                var dataOffsetRef = dataRef + dataOffset;

                if (!CryptoApi.CryptHashData(hashHandle, dataOffsetRef, (uint)dataLength, 0))
                {
                    throw CreateWin32Error();
                }
            }
        }
    }

    public static byte[] EndHashData(SafeHashHandleImpl hashHandle)
    {
        uint dataLength = 0;

        if (!CryptoApi.CryptGetHashParam(hashHandle, Constants.HP_HASHVAL, null, ref dataLength, 0))
        {
            throw CreateWin32Error();
        }

        var data = new byte[dataLength];

        if (!CryptoApi.CryptGetHashParam(hashHandle, Constants.HP_HASHVAL, data, ref dataLength, 0))
        {
            throw CreateWin32Error();
        }

        return data;
    }

    #endregion


    public static void TryDispose(this SafeHandle handle)
    {
        if (handle != null && !handle.IsClosed)
        {
            handle.Dispose();
        }
    }

    private static CryptographicException CreateWin32Error()
    {
        return ExceptionUtility.CryptographicException(Marshal.GetLastWin32Error());
    }
}