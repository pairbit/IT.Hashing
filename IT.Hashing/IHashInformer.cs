using System.Collections.Generic;

namespace IT.Hashing;

public interface IHashInformer
{
    IReadOnlyList<HashInfo> Info { get; }
}