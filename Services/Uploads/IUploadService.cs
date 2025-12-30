namespace Registry.Services.Uploads;

public interface IUploadService
{
    string NewUploadId();
    bool IsUploadPending(string uploadId);
}