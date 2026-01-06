namespace Registry.Services.Uploads;

public sealed class UploadService : IUploadService
{
    private const string UploadIdsDirectoryPath = "./blobs/uploadids/";

    public UploadService()
    {
        EnsureDirectoryExists(UploadIdsDirectoryPath);
    }

    public string NewUploadId(string name)
    {
        string uploadId = Guid.NewGuid().ToString("D");

        string path = GetUploadIdsFilePath(name, uploadId);
        File.Create(path).Dispose();

        return uploadId;
    }

    public bool IsUploadPending(string name, string uploadId)
    {
        string path = GetUploadIdsFilePath(name, uploadId);
        return File.Exists(path);
    }

    public Task AppendChunkAsync(string name, string uploadId, Stream chunkStream, CancellationToken cancellationToken)
    {
        string path = GetUploadIdsFilePath(name, uploadId);
        using FileStream fileStream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.None);
        return chunkStream.CopyToAsync(fileStream, cancellationToken);
    }

    private static string GetUploadIdsFilePath(string @namespace, string filename) => Path.Combine(UploadIdsDirectoryPath, Path.Combine(@namespace, filename));

    private static void EnsureDirectoryExists(string path)
    {
        path = Path.GetDirectoryName(path) ?? string.Empty;
        Directory.CreateDirectory(path);
    }
}