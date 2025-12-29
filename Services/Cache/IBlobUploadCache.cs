namespace Registry.Services.Cache;

public interface IBlobUploadCache
{
    void NewUpload<T>(string key, T value);
}