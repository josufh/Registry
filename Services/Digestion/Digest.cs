using System.Text.RegularExpressions;

namespace Registry.Services.Digestion;

public sealed partial class Digest
{
    public  Algorithm Algorithm { get; private set; }
    public string Hex { get; private set; } = string.Empty;

    public Digest(
        Algorithm algorithm,
        string hex)
    {
        Algorithm = algorithm;
        Hex = hex;
    }

    public static Digest FromDigestString(string digest)
    {
        string[] parts = digest.Split(':');

        Algorithm algorithm = DetectAlgorithm(parts[0]);
        string hex = ValidateHexFormat(parts[1]);

        return new(algorithm, hex);
    }

    public static Algorithm DetectAlgorithm(string algorithmString)
    {
        if (string.Equals(algorithmString, "sha256", StringComparison.CurrentCultureIgnoreCase))
        {
            return Algorithm.Sha256;
        }
        else if (string.Equals(algorithmString, "sha512", StringComparison.CurrentCultureIgnoreCase))
        {
            return Algorithm.Sha512;
        }

        throw new NotSupportedException();
    }

    public static string ValidateHexFormat(string hex)
    {
        if (!HexValidationRegex().IsMatch(hex))
        {
            throw new FormatException();
        }

        return hex;
    }

    [GeneratedRegex("^[a-f0-9]{64}$")]
    private static partial Regex HexValidationRegex();
}