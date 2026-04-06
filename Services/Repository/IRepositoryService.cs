namespace Registry.Services.Repository;

public interface IRepositoryService
{
    Task<bool> NamespaceExists(string name);
}