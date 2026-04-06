using System.Text.RegularExpressions;

namespace Registry.Services.Validation;

public partial class ValidationService : IValidationService
{
    public bool IsValidNamespaceFormat(string name)
    {
        return NamespaceRegex().IsMatch(name);
    }

    [GeneratedRegex(@"[a-z0-9]+([._-][a-z0-9]+)*(/[a-z0-9]+([._-][a-z0-9]+)*)*")]
    private static partial Regex NamespaceRegex();
}