namespace Registry.Models;

public sealed class Blob
{
    public Stream? Stream { get; }

    public Blob(Stream stream)
    {
        Stream = stream;
    }
}