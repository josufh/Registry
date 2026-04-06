using Microsoft.AspNetCore.Mvc;
using Registry.Models;
using Registry.Services.Digestion;
using Registry.Services.Uploads;
using Registry.Services.Validation;

namespace Registry.Controllers;

[ApiController]
public partial class RegistryApiController(
    IDigester digester,
    IUploadService uploadService,
    IValidationService validation) : ControllerBase
{
    [HttpGet("v2")]
    public IActionResult Base()
    {
        return Ok();
    }

    [HttpGet("v2/{name}/tags/list")]
    public async Task<IActionResult> FetchTags(
        string name,
        CancellationToken cancellationToken)
    {
        if (!validation.IsValidNamespaceFormat(name))
        {
            return NameInvalid();
        }

        return Ok();
    }

    // Monolithic POST-PUT / Chunked Upload
    [HttpPost("v2/{name}/blobs/uploads")]
    public IActionResult UploadStart(
        string name)
    {
        string uploadId = uploadService.NewUploadId(name);
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
        if (!uploadService.IsUploadPending(name, uploadId))
        {
            return BadRequest();
        }

        await uploadService.AppendChunkAsync(name, uploadId, blob.Stream, cancellationToken);

        return Ok();
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

        if (!digester.ValidateBytes(blobBytes, digest))
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

    // [HttpPost("v2/{name}/blobs/uploads/")]
    // public async Task<IActionResult> SingleUpload(
    //     string name,
    //     [FromQuery(Name = "digest")] string digestString,
    //     CancellationToken cancellationToken)
    // {
    //     if (string.IsNullOrWhiteSpace(digestString))
    //     {
    //         return BadRequest();
    //     }

    //     using MemoryStream blobStream = new();
    //     await Request.Body.CopyToAsync(blobStream, cancellationToken);
    //     byte[] blobBytes = blobStream.ToArray();

    //     Digest digest = Digest.FromDigestString(digestString);

    //     if (!digester.ValidateBytes(blobBytes, digest))
    //     {
    //         return BadRequest();
    //     }

    //     string blobKey = $"blobs/{digest.Algorithm}/{digest.Hex}";
    //     using FileStream blobFile = System.IO.File.OpenWrite(blobKey);
    //     await blobFile.WriteAsync(blobBytes, cancellationToken);
        
    //     string blobLocation = $"/v2/{name}/blobs/{digest}";

    //     Response.Headers.Location = blobLocation;
    //     Response.Headers["Docker-Content-Digest"] = $"{digest}";

    //     return Created();
    // }
}