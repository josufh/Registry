namespace Registry.Services.Validation;

public interface IValidationService
{
    bool IsValidNamespaceFormat(string name);
}