namespace Registry.Models;

public sealed class Blob
{
    public Stream? Stream { get; private set; }

    public void SetStream(Stream stream)
    {
        Stream = stream;
    }
}