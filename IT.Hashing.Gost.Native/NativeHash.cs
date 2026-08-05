using IT.Hashing.Gost.Native.Internal;

namespace IT.Hashing.Gost.Native;

public static class NativeHash
{
    private static SafeProvHandleImpl ProviderHandle => CryptoApiHelper.GetProviderHandle(GostCryptoConfig.ProviderType);

    public static INativeHash GetGost_R3411_94() => CryptoApiHelper.CreateHash_3411_94(ProviderHandle);

    public static INativeHash GetGost_R3411_2012_256() => CryptoApiHelper.CreateHash_3411_2012_256(ProviderHandle);

    public static INativeHash GetGost_R3411_2012_512() => CryptoApiHelper.CreateHash_3411_2012_512(ProviderHandle);
}