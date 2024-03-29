﻿using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace IT.Hashing.Gost.Native.Internal;

internal class LinuxVipNetNativeApi
{
    #region Для работы с криптографическим провайдером

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern bool CryptAcquireContext([In][Out] ref SafeProvHandleImpl hProv, [In] IntPtr pszContainer, [In] IntPtr pszProvider, [In] uint dwProvType, [In] uint dwFlags);

    [return: MarshalAs(UnmanagedType.Bool)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern bool CryptReleaseContext(IntPtr hCryptProv, uint dwFlags);

    [return: MarshalAs(UnmanagedType.Bool)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", CharSet = CharSet.Ansi, SetLastError = true)]
    public static extern bool CryptContextAddRef([In] IntPtr hProv, [In] byte[]? pdwReserved, [In] uint dwFlags);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern bool CryptSetProvParam([In] SafeProvHandleImpl hProv, [In] uint dwParam, [In] IntPtr pbData, [In] uint dwFlags);

    [return: MarshalAs(UnmanagedType.Bool)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", EntryPoint = "CryptSetProvParam", SetLastError = true)]
    public static extern bool CryptSetProvParam2(IntPtr hCryptProv, [In] uint dwParam, [In] byte[]? pbData, [In] uint dwFlags);

    #endregion

    #region Для работы с функцией хэширования криптографического провайдера

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern bool CryptCreateHash([In] SafeProvHandleImpl hProv, [In] uint Algid, [In] SafeKeyHandleImpl hKey, [In] uint dwFlags, [In][Out] ref SafeHashHandleImpl phHash);

    [return: MarshalAs(UnmanagedType.Bool)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern bool CryptDestroyHash(IntPtr pHashCtx);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern bool CryptGetHashParam([In] SafeHashHandleImpl hHash, [In] uint dwParam, [In][Out] byte[]? pbData, ref uint pdwDataLen, [In] uint dwFlags);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern bool CryptHashData([In] SafeHashHandleImpl hHash, [In][Out] byte[] pbData, [In] uint dwDataLen, [In] uint dwFlags);

    [return: MarshalAs(UnmanagedType.Bool)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern unsafe bool CryptHashData([In] SafeHashHandleImpl hHash, byte* pbData, [In] uint dwDataLen, [In] uint dwFlags);

    #endregion

    #region Для работы с ключами криптографического провайдера

    [return: MarshalAs(UnmanagedType.Bool)]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [DllImport("/opt/itcs/lib/libadvapi32.so", SetLastError = true)]
    public static extern bool CryptDestroyKey(IntPtr pKeyCtx);

    #endregion
}