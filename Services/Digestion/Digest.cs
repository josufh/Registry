using System.Diagnostics;
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
        string hex = ValidateHexFormat(algorithm, parts[1]);

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

    public static string ValidateHexFormat(Algorithm algorithm, string hex)
    {
        Regex validationRegex = algorithm switch
        {
            Algorithm.Sha256 => Hex256ValidationRegex(),
            Algorithm.Sha512 => Hex512ValidationRegex(),
            _ => throw new UnreachableException()
        };

        if (!validationRegex.IsMatch(hex))
        {
            throw new FormatException();
        }

        return hex;
    }

    [GeneratedRegex("^[a-f0-9]{64}$")]
    private static partial Regex Hex256ValidationRegex();

    [GeneratedRegex("^[a-f0-9]{128}$")]
    private static partial Regex Hex512ValidationRegex();

    public override string ToString()
    {
        return $"{Algorithm}:{Hex}";
    }
}