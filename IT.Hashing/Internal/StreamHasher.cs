using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing.Internal;

internal static class StreamHasher
{
    private const int BufferLength = 4096;

    public static void Append(Stream stream, Action<byte[], int, int> append)
    {
        var pool = ArrayPool<byte>.Shared;
        byte[] rented = pool.Rent(BufferLength);

        int readed;
        int clearLimit = 0;

        while ((readed = stream.Read(rented, 0, rented.Length)) > 0)
        {
            if (readed > clearLimit) clearLimit = readed;

            append(rented, 0, readed);
        }
#if NETSTANDARD2_0
        Array.Clear(rented, 0, clearLimit);
#else
        System.Security.Cryptography.CryptographicOperations.ZeroMemory(rented.AsSpan(0, clearLimit));
#endif
        pool.Return(rented, clearArray: false);
    }

    public static async Task AppendAsync(Stream stream, Action<byte[], int, int> append, CancellationToken token = default)
    {
        var pool = ArrayPool<byte>.Shared;
        byte[] rented = pool.Rent(BufferLength);
        int readed;
        int clearLimit = 0;

#if NETSTANDARD2_0
        while ((readed = await stream.ReadAsync(rented, 0, rented.Length, token).ConfigureAwait(false)) > 0)
        {
            if (readed > clearLimit) clearLimit = readed;

            append(rented, 0, readed);
        }
        Array.Clear(rented, 0, clearLimit);
#else
        Memory<byte> buffer = rented;

        while ((readed = await stream.ReadAsync(buffer, token).ConfigureAwait(false)) > 0)
        {
            if (readed > clearLimit) clearLimit = readed;

            append(rented, 0, readed);
        }

        System.Security.Cryptography.CryptographicOperations.ZeroMemory(rented.AsSpan(0, clearLimit));
#endif
        pool.Return(rented, clearArray: false);
    }
}