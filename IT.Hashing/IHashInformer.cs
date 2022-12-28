using System.Collections.Generic;

namespace IT.Hashing;

public interface IHashInformer
{
    IReadOnlyCollection<HashInfo> Info { get; }
}