using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IT.Hashing;

public interface IHasher64 : IHasher, ISpanHasher64
{
    new IHashAlg64 GetAlg(string? name = null);

    ulong Hash64(Stream stream, string? name = null);

    Task<ulong> Hash64Async(Stream stream, string? name = null, CancellationToken token = default);
}