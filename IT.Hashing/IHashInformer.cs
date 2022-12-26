namespace IT.Hashing;

public interface IHashInformer
{
    string? GetOid(string alg);

    int GetSize(string alg);
}