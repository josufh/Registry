using Registry.Services.Digestion;

namespace Registry.Services.Storage;

public sealed class BlobStorage : IBlobStorage
{
    private readonly string _rootPath;
    private readonly IDigester _digester;

    public bool HasBlob { get; private set; }
    public string? Algorithm { get; private set; }
}