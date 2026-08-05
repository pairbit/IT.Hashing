using System.Security;

namespace IT.Hashing.Gost.Native;

using Internal;

/// <summary>
/// Реализация алгоритма хэширования ГОСТ Р 34.11-2012/256.
/// </summary>
public sealed class Gost_R3411_2012_256_HashAlgorithm : Gost_R3411_HashAlgorithm
{
	/// <summary>
	/// Размер хэша ГОСТ Р 34.11-2012/256.
	/// </summary>
	public const int DefaultHashSizeValue = 256;

	/// <summary>
	/// Наименование алгоритма хэширования ГОСТ Р 34.11-2012/256.
	/// </summary>
	public const string AlgorithmNameValue = "urn:ietf:params:xml:ns:cpxmlsec:algorithms:gostr34112012-256";

	/// <summary>
	/// Известные наименования алгоритма хэширования ГОСТ Р 34.11-2012/256.
	/// </summary>
	public static readonly string[] KnownAlgorithmNames = { AlgorithmNameValue };


	/// <inheritdoc />
	[SecuritySafeCritical]
	public Gost_R3411_2012_256_HashAlgorithm() : base(DefaultHashSizeValue)
	{
	}

	/// <inheritdoc />
	[SecuritySafeCritical]
	public Gost_R3411_2012_256_HashAlgorithm(ProviderType providerType) : base(providerType, DefaultHashSizeValue)
	{
	}

	[SecurityCritical]
	internal Gost_R3411_2012_256_HashAlgorithm(ProviderType providerType, SafeProvHandleImpl providerHandle) : base(providerType, providerHandle, DefaultHashSizeValue)
	{
	}

	/// <inheritdoc />
	public override string AlgorithmName => AlgorithmNameValue;

	/// <inheritdoc />
	[SecurityCritical]
	internal override SafeHashHandleImpl CreateHashHandle(SafeProvHandleImpl providerHandle)
	{
		return CryptoApiHelper.CreateHash_3411_2012_256(providerHandle);
	}
}