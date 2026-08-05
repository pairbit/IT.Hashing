using System.Security;

namespace IT.Hashing.Gost.Native;

using Internal;
using System;

/// <summary>
/// Базовый класс для всех реализаций алгоритма хэширования ГОСТ Р 34.11.
/// </summary>
public abstract class Gost_R3411_HashAlgorithm : GostHashAlgorithm
{
	/// <inheritdoc />
	[SecuritySafeCritical]
	protected Gost_R3411_HashAlgorithm(int hashSize) : base(hashSize)
	{
		_hashHandle = CreateHashHandle();
	}

	/// <inheritdoc />
	[SecuritySafeCritical]
	protected Gost_R3411_HashAlgorithm(ProviderType providerType, int hashSize) : base(providerType, hashSize)
	{
		_hashHandle = CreateHashHandle();
	}

	[SecurityCritical]
	internal Gost_R3411_HashAlgorithm(ProviderType providerType, SafeProvHandleImpl providerHandle, int hashSize) : base(providerType, hashSize)
	{
		_hashHandle = CreateHashHandle(providerHandle);
	}

	/// <summary>
	/// Создает дескриптор функции хэширования криптографического провайдера.
	/// </summary>
	[SecurityCritical]
    internal SafeHashHandleImpl CreateHashHandle()
	{
		return CreateHashHandle(CryptoApiHelper.GetProviderHandle(ProviderType));
	}

	/// <summary>
	/// Создает дескриптор функции хэширования криптографического провайдера.
	/// </summary>
	[SecurityCritical]
    internal abstract SafeHashHandleImpl CreateHashHandle(SafeProvHandleImpl providerHandle);

	[SecurityCritical]
	private SafeHashHandleImpl _hashHandle;

    [SecuritySafeCritical]
    public void HashData(byte[] data, int dataOffset, int dataLength)
    {
        CryptoApiHelper.HashData(_hashHandle, data, dataOffset, dataLength);
    }

    [SecuritySafeCritical]
    public void HashData(ReadOnlySpan<byte> data)
    {
        CryptoApiHelper.HashData(_hashHandle, data);
    }

    [SecuritySafeCritical]
    public int GetHashFinalLength()
    {
        return CryptoApiHelper.GetEndHashDataLength(_hashHandle);
    }

    [SecuritySafeCritical]
    public int HashFinal(byte[] hash)
    {
		return CryptoApiHelper.EndHashData(_hashHandle, hash);
    }

    [SecuritySafeCritical]
    public int HashFinal(Span<byte> hash)
    {
        return CryptoApiHelper.EndHashData(_hashHandle, hash);
    }

    /// <inheritdoc />
    [SecuritySafeCritical]
	public override void Initialize()
	{
		_hashHandle.TryDispose();
		_hashHandle = CreateHashHandle();
	}

	/// <inheritdoc />
	[SecuritySafeCritical]
	protected override void HashCore(byte[] data, int dataOffset, int dataLength)
	{
		CryptoApiHelper.HashData(_hashHandle, data, dataOffset, dataLength);
	}

	/// <inheritdoc />
	[SecuritySafeCritical]
	protected override byte[] HashFinal()
	{
		return CryptoApiHelper.EndHashData(_hashHandle);
	}

	/// <inheritdoc />
	[SecuritySafeCritical]
	protected override void Dispose(bool disposing)
	{
		_hashHandle.TryDispose();

		base.Dispose(disposing);
	}
}