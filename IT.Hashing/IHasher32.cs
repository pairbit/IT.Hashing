using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher32 : IHasher, ISpanHasher32
{
    new IHashAlgorithm32 GetAlgorithm();

    new IHashAlgorithm32 GetAlgorithm(string? alg);

    uint Hash32(Stream stream);

    uint Hash32(Stream stream, string? alg);

    Task<uint> Hash32Async(Stream stream, CancellationToken token = default);

    Task<uint> Hash32Async(Stream stream, string? alg, CancellationToken token = default);
}