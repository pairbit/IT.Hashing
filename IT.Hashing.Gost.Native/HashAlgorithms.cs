using IT.Hashing.Gost.Native.Internal;
using System;

namespace IT.Hashing.Gost.Native;

public static class HashAlgorithms
{
    private readonly static SafeProvHandleImpl? _provider;
    private readonly static ProviderType? _providerType;

    public static ProviderType? NativeProviderType => _providerType;

    static HashAlgorithms()
    {
        if (CryptoApiHelper.TryGetProviderHandle(ProviderType.CryptoPro, out var provider))
        {
            _provider = provider;
            _providerType = ProviderType.CryptoPro;
        }
        else if (CryptoApiHelper.TryGetProviderHandle(ProviderType.VipNet, out provider))
        {
            _provider = provider;
            _providerType = ProviderType.VipNet;
        }
    }

    public static IHashAlgorithm CreateNativeGost3411_94(bool resetable = true) => _provider != null
        ? (resetable ? new Resetable_Gost3411_94() : CryptoApiHelper.CreateHash_3411_94(_provider))
        : throw CryptoProviderNotFound();

    public static IHashAlgorithm CreateNativeGost3411_2012_256(bool resetable = true) => _provider != null
        ? (resetable ? new Resetable_Gost3411_2012_256() : CryptoApiHelper.CreateHash_3411_2012_256(_provider))
        : throw CryptoProviderNotFound();

    public static IHashAlgorithm CreateNativeGost3411_2012_512(bool resetable = true) => _provider != null
        ? (resetable ? new Resetable_Gost3411_2012_512() : CryptoApiHelper.CreateHash_3411_2012_512(_provider))
        : throw CryptoProviderNotFound();

    //public static IHashAlgorithm CreateGost3411_94(bool resetable = true) => _provider != null
    //    ? (resetable ? new Resetable_Gost3411_94() : CryptoApiHelper.CreateHash_3411_94(_provider))
    //    : throw new NotImplementedException();

    public static IHashAlgorithm CreateGost3411_2012_256(bool resetable = true) => _provider != null
        ? (resetable ? new Resetable_Gost3411_2012_256() : CryptoApiHelper.CreateHash_3411_2012_256(_provider))
        : new Gost3411_2012_256();

    public static IHashAlgorithm CreateGost3411_2012_512(bool resetable = true) => _provider != null
        ? (resetable ? new Resetable_Gost3411_2012_512() : CryptoApiHelper.CreateHash_3411_2012_512(_provider))
        : new Gost3411_2012_512();

    private static InvalidOperationException CryptoProviderNotFound() => new("CryptoProvider not found.");

    private abstract class Resetable_Gost3411 : IHashAlgorithm
    {
        private SafeHashHandleImpl _handle;

        public virtual int Size => _handle.Size;

        protected Resetable_Gost3411()
        {
            _handle = CreateHandle();
        }

        public void Append(byte value)
            => _handle.Append(value);

        public void Append(ReadOnlySpan<byte> span)
            => _handle.Append(span);

        public void Append(byte[] array, int start, int length)
            => _handle.Append(array, start, length);

        public void Dispose()
            => _handle.TryDispose();

        public void Reset()
        {
            _handle.TryDispose();
            _handle = CreateHandle();
        }

        public bool TryGetHash(Span<byte> hash, out int written)
            => _handle.TryGetHash(hash, out written);

        protected abstract SafeHashHandleImpl CreateHandle();
    }

    private class Resetable_Gost3411_94 : Resetable_Gost3411
    {
        public override int Size => 32;

        protected override SafeHashHandleImpl CreateHandle() =>
            CryptoApiHelper.CreateHash_3411_94(_provider!);
    }

    private class Resetable_Gost3411_2012_256 : Resetable_Gost3411
    {
        public override int Size => 32;

        protected override SafeHashHandleImpl CreateHandle() =>
            CryptoApiHelper.CreateHash_3411_2012_256(_provider!);
    }

    private class Resetable_Gost3411_2012_512 : Resetable_Gost3411
    {
        public override int Size => 64;

        protected override SafeHashHandleImpl CreateHandle() =>
            CryptoApiHelper.CreateHash_3411_2012_512(_provider!);
    }
}