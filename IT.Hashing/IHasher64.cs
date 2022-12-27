using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher64 : IHasher, ISpanHasher64
{
    new IHashAlgorithm64 GetAlgorithm();

    new IHashAlgorithm64 GetAlgorithm(string? alg);

    ulong Hash64(Stream stream);

    ulong Hash64(Stream stream, string? alg);

    Task<ulong> Hash64Async(Stream stream, CancellationToken token = default);

    Task<ulong> Hash64Async(Stream stream, string? alg, CancellationToken token = default);
}