namespace IT.Hashing;

public interface IHashInformer
{
    int GetSize();

    int GetSize(string? alg);

    string GetName();

    string GetName(string? alg);

    string? GetOid();

    string? GetOid(string? alg);
}