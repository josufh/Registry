namespace Registry.Services.Storage;

public interface IBlobStorage
{
    Task SaveAsync(string key, Stream content);
}