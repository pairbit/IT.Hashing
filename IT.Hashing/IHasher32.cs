using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher32 : IHasher, ISpanHasher32
{
    new IHashAlg32 GetAlg(string? name = null);

    uint Hash32(Stream stream, string? name = null);

    Task<uint> Hash32Async(Stream stream, string? name = null, CancellationToken token = default);
}