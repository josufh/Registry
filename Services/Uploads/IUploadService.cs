namespace Registry.Services.Uploads;

public interface IUploadService
{
    string NewUploadId(string name);
    bool IsUploadPending(string name, string uploadId);
    Task AppendChunkAsync(string name, string uploadId, Stream chunkStream, CancellationToken cancellationToken);
}