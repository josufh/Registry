namespace Registry.Services.Uploads;

public sealed class UploadService
{
    private const string UploadIdsDirectoryPath = "./blobs/uploadids/";

    public UploadService()
    {
        EnsureDirectoryExists(UploadIdsDirectoryPath);
    }

    public string NewUploadId()
    {
        string uploadId = Guid.NewGuid().ToString("D");

        string path = GetUploadIdsFilePath(uploadId);
        File.Create(path).Dispose();

        return uploadId;
    }

    public bool IsUploadPending(string uploadId)
    {
        string path = GetUploadIdsFilePath(uploadId);
        return File.Exists(path);
    }

    private static string GetUploadIdsFilePath(string filename) => Path.Combine(UploadIdsDirectoryPath, filename);

    private static void EnsureDirectoryExists(string path)
    {
        path = Path.GetDirectoryName(path) ?? string.Empty;
        Directory.CreateDirectory(path);
    }
}