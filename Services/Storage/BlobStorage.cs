using Registry.Services.Digestion;

namespace Registry.Services.Storage;

public sealed class BlobStorage : IBlobStorage
{
    public async Task SaveAsync(string key, Stream blobStream)
    {
        EnsureDirectory(key);

        using FileStream blobFile = File.Open(key, FileMode.Create, FileAccess.Write, FileShare.None);
        await blobStream.CopyToAsync(blobFile);
    }

    private static void EnsureDirectory(string path)
    {
        string dirPath = Path.GetDirectoryName(path)!;
        Directory.CreateDirectory(dirPath);
    }
}