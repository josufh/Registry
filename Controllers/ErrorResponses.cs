using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Registry.Constants;

namespace Registry.Controllers;

public partial class RegistryApiController
{
    private IActionResult NameInvalid()
    {
        string body = BuildError(ErrorCodes.NameInvalid);

        return new ContentResult
        {
            StatusCode = StatusCodes.Status400BadRequest,
            ContentType = MediaTypeNames.Application.Json,
            Content = body
        };
    }

    private string BuildError(params string[] errorCodes)
    {
        IEnumerable<object> errors = errorCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => new { code });

        object payload = new
        {
            errors
        };

        return JsonSerializer.Serialize(payload);
    }
}