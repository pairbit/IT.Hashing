//using System;
//using System.Buffers;
//using System.IO;
//using System.Security.Cryptography;
//using System.Threading;
//using System.Threading.Tasks;

//namespace IT.Hashing;

//public class HashAlg : IHashAlg
//{
//    private HashAlgorithm _algorithm;

//    public HashInfo Info => throw new NotImplementedException();

//    public HashAlg(HashAlgorithm algorithm)
//    {
//        _algorithm = algorithm;
//    }

//    public void Append(ReadOnlySpan<byte> bytes)
//    {
//        var pool = ArrayPool<byte>.Shared;

//        byte[] rented = pool.Rent(bytes.Length);

//        bytes.CopyTo(rented);

//        _algorithm.TransformBlock(rented, 0, bytes.Length, null, 0);

//        Array.Clear(rented, 0, bytes.Length);

//        pool.Return(rented);
//    }

//    public void Append(Stream stream)
//    {
//        throw new NotImplementedException();
//    }

//    public Task AppendAsync(Stream stream, CancellationToken token = default)
//    {
//        throw new NotImplementedException();
//    }

//    public void Reset()
//    {
//        _algorithm.Initialize();
//    }

//    public int TryHash(Span<byte> hash)
//    {
//        throw new NotImplementedException();
//    }
//}