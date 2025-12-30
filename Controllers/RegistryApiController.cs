using Microsoft.AspNetCore.Mvc;
using Registry.Models;
using Registry.Services.Digestion;
using Registry.Services.Uploads;

namespace Registry.Controllers;

[ApiController]
public sealed class RegistryApiController : ControllerBase
{
    private readonly IDigester _digester;
    private readonly IUploadService _uploadService;

    public RegistryApiController(
        IDigester digester,
        IUploadService uploadService)
    {
        _digester = digester;
        _uploadService = uploadService;
    }

    [HttpGet("v2")]
    public IActionResult Ping()
    {
        return Ok();
    }

    // Monolithic POST-PUT / Chunked Upload
    [HttpPost("v2/{name}/blobs/uploads")]
    public IActionResult UploadStart(
        string name)
    {
        string uploadId = _uploadService.NewUploadId();
        string location = $"/v2/{name}/blobs/uploads/{uploadId}";

        Response.Headers.Location = location;
        
        return Accepted();
    }

    [HttpPatch("v2/{name}/blobs/uploads/{uploadId}")]
    public async Task<IActionResult> UploadChunk(
        string name,
        string uploadId,
        [FromServices] Blob blob,
        CancellationToken cancellationToken)
    {
        
    }

    [HttpPut("v2/{name}/blobs/uploads/{uploadId}")]
    public async Task<IActionResult> UploadComplete(
        string name,
        string uploadId,
        [FromQuery(Name = "digest")] string digestString,
        [FromServices] Blob blob,
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

        string blobKey = $"blobs/{digest.Algorithm}/{digest.Hex}";
        using FileStream blobFile = System.IO.File.OpenWrite(blobKey);
        await blobFile.WriteAsync(blobBytes, cancellationToken);
        
        string blobLocation = $"/v2/{name}/blobs/{digest}";

        Response.Headers.Location = blobLocation;
        Response.Headers["Docker-Content-Digest"] = $"{digest}";

        return Created();
    }

    [HttpPost("v2/{name}/blobs/uploads/")]
    public async Task<IActionResult> SingleUpload(
        string name,
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

        string blobKey = $"blobs/{digest.Algorithm}/{digest.Hex}";
        using FileStream blobFile = System.IO.File.OpenWrite(blobKey);
        await blobFile.WriteAsync(blobBytes, cancellationToken);
        
        string blobLocation = $"/v2/{name}/blobs/{digest}";

        Response.Headers.Location = blobLocation;
        Response.Headers["Docker-Content-Digest"] = $"{digest}";

        return Created();
    }
}