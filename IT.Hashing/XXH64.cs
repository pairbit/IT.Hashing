﻿using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public unsafe class XXH64 : XXH, IHashAlg64
{
    internal const ulong PRIME64_1 = 11400714785074694791ul;
    internal const ulong PRIME64_2 = 14029467366897019727ul;
    internal const ulong PRIME64_3 = 1609587929392839161ul;
    internal const ulong PRIME64_4 = 9650029242287828579ul;
    internal const ulong PRIME64_5 = 2870177450012600261ul;
    private XXH64_state _state;
    public const ulong EmptyHash = 17241709254077376921;
    public const int HashSizeInBits = 64;
    public const int HashSizeInBytes = HashSizeInBits / 8;
    public static readonly HashInfo HashInfo = new(typeof(XXH64).FullName!, "XXH64", HashSizeInBytes, null);

    [StructLayout(LayoutKind.Sequential)]
    private struct XXH64_state
    {
        public ulong total_len;
        public ulong v1;
        public ulong v2;
        public ulong v3;
        public ulong v4;
        public fixed ulong mem64[4];
        public uint memsize;
    }

    #region Public

    public override HashInfo Info => HashInfo;

    public XXH64()
    {
        Reset();
    }

    public static HashAlgorithm Create() => new Internal.HashAlgorithmAdapter(new XXH64());

    public static int TryHash(ReadOnlySpan<byte> bytes, Span<byte> hash)
    {
        if (hash.Length < sizeof(ulong)) return 0;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(hash), Hash64(bytes));

        return HashSizeInBytes;
    }

    public static byte[] Hash(ReadOnlySpan<byte> bytes)
    {
        byte[] hash = new byte[sizeof(ulong)];
        Unsafe.As<byte, ulong>(ref hash[0]) = Hash64(bytes);
        return hash;
    }

    public static int Hash(Stream stream, Span<byte> hash) => throw new NotImplementedException();

    public static ValueTask<int> HashAsync(Stream stream, Memory<byte> hash, CancellationToken token = default)
        => throw new NotImplementedException();

    public static unsafe ulong Hash64(void* bytes, int length) => XXH64_hash(bytes, length, 0);

    public static unsafe ulong Hash64(ReadOnlySpan<byte> bytes)
    {
        fixed (byte* bytesP = bytes)
            return Hash64(bytesP, bytes.Length);
    }

    public static unsafe ulong Hash64(byte[] bytes, int offset, int length)
    {
        Validate(bytes, offset, length);

        fixed (byte* bytes0 = bytes)
            return Hash64(bytes0 + offset, length);
    }

    public static ulong Hash64(Stream stream) => throw new NotImplementedException();

    public static ValueTask<ulong> Hash64Async(Stream stream, CancellationToken token = default) => throw new NotImplementedException();

    public override unsafe void Reset()
    {
        fixed (XXH64_state* stateP = &_state)
            XXH64_reset(stateP, 0);
    }

    public unsafe void Append(byte* bytes, int length)
    {
        fixed (XXH64_state* stateP = &_state)
            XXH64_update(stateP, bytes, length);
    }

    public override unsafe void Append(ReadOnlySpan<byte> bytes)
    {
        fixed (byte* bytesP = bytes)
            Append(bytesP, bytes.Length);
    }

    public override unsafe void Append(byte[] bytes, int offset, int length)
    {
        Validate(bytes, offset, length);

        fixed (byte* bytesP = bytes)
            Append(bytesP + offset, length);
    }

    public unsafe ulong GetHash64()
    {
        fixed (XXH64_state* stateP = &_state)
            return XXH64_digest(stateP);
    }

    public override byte[] GetHash()
    {
        byte[] bytes = new byte[sizeof(ulong)];
        Unsafe.As<byte, ulong>(ref bytes[0]) = GetHash64();
        return bytes;
    }

    public override int TryGetHash(Span<byte> hash)
    {
        if (hash.Length < sizeof(ulong)) return 0;

        Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(hash), GetHash64());

        return sizeof(ulong);
    }

    #endregion

    #region Private

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong XXH_rotl64(ulong x, int r) => x << r | x >> 64 - r;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong XXH64_round(ulong acc, ulong input) => XXH_rotl64(acc + input * PRIME64_2, 31) * PRIME64_1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong XXH64_mergeRound(ulong acc, ulong val) => (acc ^ XXH64_round(0, val)) * PRIME64_1 + PRIME64_4;

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    private static ulong XXH64_hash(void* input, int len, ulong seed)
    {
        var p = (byte*)input;
        var bEnd = p + len;
        ulong h64;

        if (len >= 32)
        {
            var limit = bEnd - 32;
            var v1 = seed + PRIME64_1 + PRIME64_2;
            var v2 = seed + PRIME64_2;
            var v3 = seed + 0;
            var v4 = seed - PRIME64_1;

            do
            {
                v1 = XXH64_round(v1, XXH_read64(p + 0));
                v2 = XXH64_round(v2, XXH_read64(p + 8));
                v3 = XXH64_round(v3, XXH_read64(p + 16));
                v4 = XXH64_round(v4, XXH_read64(p + 24));
                p += 32;
            }
            while (p <= limit);

            h64 = XXH_rotl64(v1, 1) + XXH_rotl64(v2, 7) + XXH_rotl64(v3, 12) + XXH_rotl64(v4, 18);
            h64 = XXH64_mergeRound(h64, v1);
            h64 = XXH64_mergeRound(h64, v2);
            h64 = XXH64_mergeRound(h64, v3);
            h64 = XXH64_mergeRound(h64, v4);
        }
        else
        {
            h64 = seed + PRIME64_5;
        }

        h64 += (ulong)len;

        while (p + 8 <= bEnd)
        {
            h64 ^= XXH64_round(0, XXH_read64(p));
            h64 = XXH_rotl64(h64, 27) * PRIME64_1 + PRIME64_4;
            p += 8;
        }

        if (p + 4 <= bEnd)
        {
            h64 ^= XXH_read32(p) * PRIME64_1;
            h64 = XXH_rotl64(h64, 23) * PRIME64_2 + PRIME64_3;
            p += 4;
        }

        while (p < bEnd)
        {
            h64 ^= *p * PRIME64_5;
            h64 = XXH_rotl64(h64, 11) * PRIME64_1;
            p++;
        }

        h64 ^= h64 >> 33;
        h64 *= PRIME64_2;
        h64 ^= h64 >> 29;
        h64 *= PRIME64_3;
        h64 ^= h64 >> 32;

        return h64;
    }

    private static void XXH64_reset(XXH64_state* state, ulong seed)
    {
        XXH_zero(state, sizeof(XXH64_state));
        state->v1 = seed + PRIME64_1 + PRIME64_2;
        state->v2 = seed + PRIME64_2;
        state->v3 = seed + 0;
        state->v4 = seed - PRIME64_1;
    }

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    private static void XXH64_update(XXH64_state* state, void* input, int len)
    {
        var p = (byte*)input;
        var bEnd = p + len;

        state->total_len += (ulong)len;

        if (state->memsize + len < 32)
        {
            /* fill in tmp buffer */
            XXH_copy((byte*)state->mem64 + state->memsize, input, len);
            state->memsize += (uint)len;
            return;
        }

        if (state->memsize > 0)
        {
            /* tmp buffer is full */
            XXH_copy((byte*)state->mem64 + state->memsize, input, (int)(32 - state->memsize));
            state->v1 = XXH64_round(state->v1, XXH_read64(state->mem64 + 0));
            state->v2 = XXH64_round(state->v2, XXH_read64(state->mem64 + 1));
            state->v3 = XXH64_round(state->v3, XXH_read64(state->mem64 + 2));
            state->v4 = XXH64_round(state->v4, XXH_read64(state->mem64 + 3));
            p += 32 - state->memsize;
            state->memsize = 0;
        }

        if (p + 32 <= bEnd)
        {
            var limit = bEnd - 32;
            var v1 = state->v1;
            var v2 = state->v2;
            var v3 = state->v3;
            var v4 = state->v4;

            do
            {
                v1 = XXH64_round(v1, XXH_read64(p + 0));
                v2 = XXH64_round(v2, XXH_read64(p + 8));
                v3 = XXH64_round(v3, XXH_read64(p + 16));
                v4 = XXH64_round(v4, XXH_read64(p + 24));
                p += 32;
            }
            while (p <= limit);

            state->v1 = v1;
            state->v2 = v2;
            state->v3 = v3;
            state->v4 = v4;
        }

        if (p < bEnd)
        {
            XXH_copy(state->mem64, p, (int)(bEnd - p));
            state->memsize = (uint)(bEnd - p);
        }
    }

#if NET5_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
#endif
    private static ulong XXH64_digest(XXH64_state* state)
    {
        var p = (byte*)state->mem64;
        var bEnd = (byte*)state->mem64 + state->memsize;
        ulong h64;

        if (state->total_len >= 32)
        {
            var v1 = state->v1;
            var v2 = state->v2;
            var v3 = state->v3;
            var v4 = state->v4;

            h64 = XXH_rotl64(v1, 1) + XXH_rotl64(v2, 7) + XXH_rotl64(v3, 12) + XXH_rotl64(v4, 18);
            h64 = XXH64_mergeRound(h64, v1);
            h64 = XXH64_mergeRound(h64, v2);
            h64 = XXH64_mergeRound(h64, v3);
            h64 = XXH64_mergeRound(h64, v4);
        }
        else
        {
            h64 = state->v3 + PRIME64_5;
        }

        h64 += state->total_len;

        while (p + 8 <= bEnd)
        {
            h64 ^= XXH64_round(0, XXH_read64(p));
            h64 = XXH_rotl64(h64, 27) * PRIME64_1 + PRIME64_4;
            p += 8;
        }

        if (p + 4 <= bEnd)
        {
            h64 ^= XXH_read32(p) * PRIME64_1;
            h64 = XXH_rotl64(h64, 23) * PRIME64_2 + PRIME64_3;
            p += 4;
        }

        while (p < bEnd)
        {
            h64 ^= *p * PRIME64_5;
            h64 = XXH_rotl64(h64, 11) * PRIME64_1;
            p++;
        }

        h64 ^= h64 >> 33;
        h64 *= PRIME64_2;
        h64 ^= h64 >> 29;
        h64 *= PRIME64_3;
        h64 ^= h64 >> 32;

        return h64;
    }

    #endregion Private
}