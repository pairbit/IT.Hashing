using System.Security;
using System.Security.Cryptography;

namespace IT.Hashing.Gost.Native;

/// <summary>
/// Базовый класс для всех алгоритмов хэширования ГОСТ.
/// </summary>
public abstract class GostHashAlgorithm : HashAlgorithm
{
	/// <summary>
	/// Конструктор.
	/// </summary>
	/// <param name="hashSize">Размер хэш-кода в битах.</param>
	/// <remarks>
	/// По умолчанию использует криптографический провайдер, установленный в <see cref="GostCryptoConfig.ProviderType"/>.
	/// </remarks>
	[SecuritySafeCritical]
	protected GostHashAlgorithm(int hashSize) : this(GostCryptoConfig.ProviderType, hashSize)
	{
	}

	/// <summary>
	/// Конструктор.
	/// </summary>
	/// <param name="providerType">Тип криптографического провайдера.</param>
	/// <param name="hashSize">Размер хэш-кода в битах.</param>
	[SecuritySafeCritical]
	protected GostHashAlgorithm(ProviderType providerType, int hashSize)
	{
		ProviderType = providerType;
		HashSizeValue = hashSize;
	}


	/// <inheritdoc />
	public ProviderType ProviderType { get; }

	/// <inheritdoc />
	public abstract string AlgorithmName { get; }
}