namespace Registry.Middleware;

public sealed class NamespaceEscapingMiddleware
{
    private static readonly HashSet<string> SuffixRoots =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "tags",
            "blobs",
            "manifests"
        };

    private readonly RequestDelegate _next;

    public NamespaceEscapingMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        PathString originalPath = context.Request.Path;
        Console.WriteLine(originalPath.Value);

        if (!originalPath.HasValue)
        {
            await _next(context);
            return;
        }

        string pathValue = originalPath.Value;
        if (!pathValue.StartsWith("/v2/", StringComparison.Ordinal))
        {
            await _next(context);
            return;
        }

        string[] segments = pathValue.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 3 || !segments[0].Equals("v2", StringComparison.InvariantCultureIgnoreCase))
        {
            await _next(context);
            return;
        }

        int suffixStartIndex = -1;
        for (int i = 0; i < segments.Length; i++)
        {
            if (SuffixRoots.Contains(segments[i]))
            {
                suffixStartIndex = i;
                break;
            }
        }

        if (suffixStartIndex == -1)
        {
            await _next(context);
            return;
        }

        string[] nameSegments = segments[1..suffixStartIndex];
        string fullName = string.Join('/', nameSegments);

        string encodedName = Uri.EscapeDataString(fullName);
        Console.WriteLine(encodedName);

        List<string> newSegments =
        [
            "",
            "v2",
            encodedName,
            ..segments[suffixStartIndex..]
        ];

        string newPathValue = string.Join('/', newSegments);
        context.Request.Path = new(newPathValue);
        Console.WriteLine("\n\n" + context.Request.Path + "\n\n");

        await _next(context);
    }
}