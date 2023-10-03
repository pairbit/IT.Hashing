using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;

namespace IT.Hashing.Gost.Native;

using Internal;

/// <summary>
/// Предоставляет методы для доступа к конфигурационной информации, используемой при работе с криптографическим провайдером ГОСТ.
/// </summary>
public static class GostCryptoConfig
{
	private static Lazy<ProviderType> _providerType_2001;
	private static Lazy<ProviderType> _providerType_2012_512;
	private static Lazy<ProviderType> _providerType_2012_1024;
	private static readonly Dictionary<string, Type> NameToType = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
	private static readonly Dictionary<string, string> NameToOid = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);


	static GostCryptoConfig()
	{
		InitDefaultProviders();
		AddKnownAlgorithms();
	}

	[SecuritySafeCritical]
	private static void InitDefaultProviders()
	{
		_providerType_2001 = new Lazy<ProviderType>(CryptoApiHelper.GetAvailableProviderType_2001);
		_providerType_2012_512 = new Lazy<ProviderType>(CryptoApiHelper.GetAvailableProviderType_2012_512);
		_providerType_2012_1024 = new Lazy<ProviderType>(CryptoApiHelper.GetAvailableProviderType_2012_1024);
	}

	private static void AddKnownAlgorithms()
	{
		AddAlgorithm<Gost_R3411_94_HashAlgorithm>(Gost_R3411_94_HashAlgorithm.KnownAlgorithmNames);
		AddAlgorithm<Gost_R3411_2012_256_HashAlgorithm>(Gost_R3411_2012_256_HashAlgorithm.KnownAlgorithmNames);
		AddAlgorithm<Gost_R3411_2012_512_HashAlgorithm>(Gost_R3411_2012_512_HashAlgorithm.KnownAlgorithmNames);
	}


	/// <summary>
	/// Инициализирует конфигурацию.
	/// </summary>
	public static void Initialize()
	{
		// На самом деле инициализация происходит в статическом конструкторе
	}


	/// <summary>
	/// Возвращает или устанавливает провайдер по умолчанию для ключей ГОСТ Р 34.10-2001.
	/// </summary>
	public static ProviderType ProviderType
	{
		get => _providerType_2001.Value;
		set => _providerType_2001 = new Lazy<ProviderType>(() => value);
	}

	/// <summary>
	/// Возвращает или устанавливает провайдер по умолчанию для ключей ГОСТ Р 34.10-2012/512.
	/// </summary>
	public static ProviderType ProviderType_2012_512
	{
		get => _providerType_2012_512.Value;
		set => _providerType_2012_512 = new Lazy<ProviderType>(() => value);
	}

	/// <summary>
	/// Возвращает или устанавливает провайдер по умолчанию для ключей ГОСТ Р 34.10-2012/1024.
	/// </summary>
	public static ProviderType ProviderType_2012_1024
	{
		get => _providerType_2012_1024.Value;
		set => _providerType_2012_1024 = new Lazy<ProviderType>(() => value);
	}


	/// <summary>
	/// Добавляет связь между алгоритмом и именем.
	/// </summary>
	[SecuritySafeCritical]
	public static void AddAlgorithm<T>(params string[] names)
	{
		var type = typeof(T);

		if (names != null)
		{
			foreach (var name in names)
			{
				NameToType.Add(name, type);
				CryptoConfig.AddAlgorithm(type, name);
			}
		}

		NameToType.Add(type.Name, type);
		CryptoConfig.AddAlgorithm(type, type.Name);

		if (type.FullName != null)
		{
			NameToType.Add(type.FullName, type);
			CryptoConfig.AddAlgorithm(type, type.FullName);
		}

		if (type.AssemblyQualifiedName != null)
		{
			NameToType.Add(type.AssemblyQualifiedName, type);
			CryptoConfig.AddAlgorithm(type, type.AssemblyQualifiedName);
		}
	}

	/// <summary>
	/// Добавляет связь между алгоритмом и OID.
	/// </summary>
	[SecuritySafeCritical]
	public static void AddOID<T>(string oid, params string[] names)
	{
		var type = typeof(T);

		if (names != null)
		{
			foreach (var name in names)
			{
				NameToOid.Add(name, oid);
				CryptoConfig.AddOID(oid, name);
			}
		}

		NameToOid.Add(type.Name, oid);
		CryptoConfig.AddOID(oid, type.Name);

		if (type.FullName != null)
		{
			NameToOid.Add(type.FullName, oid);
			CryptoConfig.AddOID(oid, type.FullName);
		}

		if (type.AssemblyQualifiedName != null)
		{
			NameToOid.Add(type.AssemblyQualifiedName, oid);
			CryptoConfig.AddOID(oid, type.AssemblyQualifiedName);
		}
	}

	/// <inheritdoc cref="CryptoConfig.MapNameToOID"/>
	public static string MapNameToOID(string name)
	{
		string oid = null;

		if (!string.IsNullOrEmpty(name))
		{
			oid = CryptoConfig.MapNameToOID(name);

			if (string.IsNullOrEmpty(oid))
			{
				NameToOid.TryGetValue(name, out oid);
			}
		}

		return oid;
	}

	/// <inheritdoc cref="CryptoConfig.CreateFromName(string,object[])"/>
	public static object CreateFromName(string name, params object[] arguments)
	{
		object obj = null;

		if (!string.IsNullOrEmpty(name))
		{
			obj = CryptoConfig.CreateFromName(name, arguments);

			if (obj == null && NameToType.TryGetValue(name, out var objType))
			{
				obj = Activator.CreateInstance(objType, arguments);
			}
		}

		return obj;
	}
}