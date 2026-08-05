using IT.Hashing.Gost.Native.Internal;

namespace IT.Hashing.Gost.Native;

public static class HashAlgorithms
{
    private readonly static SafeProvHandleImpl? _handle;

    static HashAlgorithms()
    {
        if (CryptoApiHelper.TryGetProviderHandle_2001(out var handle))
        {
            _handle = handle;
        }
    }

    public static IHashAlgorithm CreateGost3411_94() => _handle != null
        ? CryptoApiHelper.CreateHash_3411_94(_handle)
        : throw new System.NotImplementedException();

    public static IHashAlgorithm CreateGost3411_2012_256() => _handle != null
        ? CryptoApiHelper.CreateHash_3411_2012_256(_handle)
        : new Gost3411_2012_256();

    public static IHashAlgorithm CreateGost3411_2012_512() => _handle != null
        ? CryptoApiHelper.CreateHash_3411_2012_512(_handle)
        : new Gost3411_2012_512();
}