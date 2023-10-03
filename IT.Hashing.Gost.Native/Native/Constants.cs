namespace GostCryptography.Native
{
	/// <summary>
	/// Константы для работы с криптографическим провайдером.
	/// </summary>
	public static class Constants
	{
		#region Идентификаторы криптографических алгоритмов ГОСТ

		/// <summary>
		/// Идентификатор алгоритма хэширования в соответствии с ГОСТ Р 34.11-94.
		/// </summary>
		public const int CALG_GR3411 = 0x801e;

		/// <summary>
		/// Идентификатор алгоритма хэширования в соответствии с ГОСТ Р 34.11-2012, длина выхода 256 бит.
		/// </summary>
		public const int CALG_GR3411_2012_256 = 0x8021;

		/// <summary>
		/// Идентификатор алгоритма хэширования в соответствии с ГОСТ Р 34.11-2012, длина выхода 512 бит.
		/// </summary>
		public const int CALG_GR3411_2012_512 = 0x8022;

		#endregion

		#region Настройки контекста криптографического провайдера

		/// <summary>
		/// Использовать ключи локальной машины.
		/// </summary>
		public const uint CRYPT_MACHINE_KEYSET = 0x20;

		/// <summary>
		/// Получить доступ к провайдеру без необходимости доступа к приватным ключам.
		/// </summary>
		public const uint CRYPT_VERIFYCONTEXT = 0xf0000000;

		#endregion

		#region Параметры криптографического провайдера

		/// <summary>
		/// Удаляет текущий контейнер с носителя.
		/// </summary>
		public const int PP_DELETE_KEYSET = 0x7d;

		#endregion


		#region Параметры функции хэширования криптографического провайдера

		/// <summary>
		/// Значение функции хэширования в little-endian порядке байт в соотвествии с типом GostR3411-94-Digest CPCMS [RFC 4490].
		/// </summary>
		public const int HP_HASHVAL = 2;

		#endregion
	}
}