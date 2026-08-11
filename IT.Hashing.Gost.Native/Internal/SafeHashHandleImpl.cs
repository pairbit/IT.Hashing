using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.Buffers.Text;
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

    public int SizeInBase64 => Base64.GetMaxEncodedToUtf8Length(Size);

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
    public bool TryGetHash(Span<byte> hash, out int length)
    {
        return CryptoApiHelper.TryGetEndHashData(this, hash, out length);
    }

    [SecurityCritical]
    public bool TryGetHashInBase64(Span<byte> hash, out int length)
    {
        if (!CryptoApiHelper.TryGetEndHashData(this, hash, out length))
        {
            length = Base64.GetMaxEncodedToUtf8Length(length);
            return false;
        }

        var status = Base64.EncodeToUtf8InPlace(hash, length, out var written);
        if (status != OperationStatus.Done)
        {
            if (status == OperationStatus.DestinationTooSmall)
            {
                length = Base64.GetMaxEncodedToUtf8Length(length);
                return false;
            }
            throw new InvalidOperationException($"Status is {status}");
        }
        length = written;
        return true;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }
}