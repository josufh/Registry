namespace Registry.Models;

public sealed class Blob
{
    public string Algorithm { get; set; }
    public string HexDigest { get; set; }
    public string FullDigest => $"{Algorithm}:{HexDigest}";
    
}