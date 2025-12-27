using Microsoft.AspNetCore.Mvc;
using Registry.Services.Digestion;

namespace Registry.Controllers;

public sealed class RegistryApiController : ControllerBase
{
    private readonly IDigester _digester;

    public RegistryApiController(
        IDigester digester)
    {
        _digester = digester;
    }

    [HttpGet("v2")]
    public IActionResult Ping()
    {
        return Ok();
    }

    [HttpPost("v2/{name}/blobs/uploads")]
    public IActionResult MonoUploadStart(string name)
    {
        string uploadId = Guid.NewGuid().ToString("D");
        // persist upload
        string location = $"/v2/{name}/blobs/uploads/{uploadId}";

        Response.Headers.Location = location;
        
        return Accepted();
    }

    [HttpPut("v2/{name}/blobs/uploads/{uploadId}")]
    public async Task<IActionResult> MonoUploadComplete(
        string name,
        string uploadId,
        [FromQuery(Name = "digest")] string digestString,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(digestString))
        {
            return BadRequest();
        }

        using MemoryStream blobStream = new();
        await Request.Body.CopyToAsync(blobStream, cancellationToken);
        byte[] blobBytes = blobStream.ToArray();

        Digest digest = Digest.FromDigestString(digestString);

        if (!_digester.ValidateBytes(blobBytes, digest))
        {
            return BadRequest();
        }

        

        return Created();
    }
}