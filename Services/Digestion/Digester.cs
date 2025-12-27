using System.Diagnostics;
using System.Security.Cryptography;

namespace Registry.Services.Digestion;

public sealed class Digester : IDigester
{
    public bool ValidateBytes(byte[] bytes, Digest expected)
    {
        HashAlgorithm hasher = expected.Algorithm switch
        {
            Algorithm.Sha256 => SHA256.Create(),
            Algorithm.Sha512 => SHA512.Create(),
            _ => throw new UnreachableException()
        };
        byte[] hash = hasher.ComputeHash(bytes);
        string actualHex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        
        return string.Equals(actualHex, expected.Hex, StringComparison.OrdinalIgnoreCase);
    }
}