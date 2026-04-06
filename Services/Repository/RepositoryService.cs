namespace Registry.Services.Repository;

public sealed class RepositoryService : IRepositoryService
{
    private const string RepositoriesDirectoryPath = "/repositories";

    public async Task<bool> NamespaceExists(string name)
    {
        string path = Path.Combine(RepositoriesDirectoryPath, name);
        bool exists = Directory.Exists(path);

        return await Task.FromResult(exists);
    }
}