using System.Collections.Generic;

namespace IT.Hashing;

public interface IHashInformer
{
    IReadOnlyCollection<string> Algs { get; }

    int GetSize();

    int GetSize(string? alg);

    string GetName();

    string GetName(string? alg);

    string? GetOid();

    string? GetOid(string? alg);
}