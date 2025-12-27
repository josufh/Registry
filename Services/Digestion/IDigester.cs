namespace Registry.Services.Digestion;

public interface IDigester
{
    // Algorithm DetectAlgorithm(string algorithmString);
    // string ValidateHex(string hex);
    bool ValidateBytes(byte[] bytes, Digest expected);
}