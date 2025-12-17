using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;
using PRN232_GradingSystem_API.Models;

namespace PRN232_GradingSystem_API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private static readonly string[] AllowedExtensions = new[] { ".zip", ".rar" };
    private const long MaxFileSizeBytes = 1024L * 1024 * 1024; // 1 GB
    private readonly IFileStorageService _fileStorageService;
    private readonly ISubmissionService _submissionService;

    public FilesController(IFileStorageService fileStorageService, ISubmissionService submissionService)
    {
        _fileStorageService = fileStorageService;
        _submissionService = submissionService;
    }

    [HttpPost("upload-archive")]
    [Consumes("multipart/form-data")]
    [DisableRequestSizeLimit]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSizeBytes, ValueLengthLimit = int.MaxValue)]
    public async Task<IActionResult> UploadArchive([FromForm] FileUploadRequest request, CancellationToken cancellationToken)
    {
        var file = request.File;
        if (file == null || file.Length == 0)
        {
            return BadRequest(ApiResponse<string>.FailResponse("File is required."));
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return BadRequest(ApiResponse<string>.FailResponse("File is too large (max 1GB)."));
        }

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
        {
            return BadRequest(ApiResponse<string>.FailResponse("Only .zip or .rar files are allowed."));
        }

        await using var stream = file.OpenReadStream();
        var urls = await _fileStorageService.UploadArchiveAsync(
            stream,
            file.FileName,
            file.ContentType ?? string.Empty,
            request.Prefix ?? string.Empty,
            request.ExamId ?? 0,
            request.EntityName,
            cancellationToken);
        return Ok(ApiResponse<IReadOnlyList<string>>.SuccessResponse(urls, "Uploaded"));
    }

    [HttpGet("submission/{submissionId:int}/download")]
    public async Task<IActionResult> DownloadSubmissionFile(int submissionId, CancellationToken cancellationToken)
    {
        var submission = await _submissionService.GetByIdAsync(submissionId);
        if (submission == null)
        {
            return NotFound(ApiResponse<string>.FailResponse($"Submission '{submissionId}' was not found."));
        }

        if (string.IsNullOrWhiteSpace(submission.Fileurl))
        {
            return BadRequest(ApiResponse<string>.FailResponse("Submission does not have an associated file."));
        }

        try
        {
            var download = await _fileStorageService.DownloadFileAsync(submission.Fileurl, cancellationToken);
            HttpContext.Response.RegisterForDispose(download);

            var suggestedName = download.FileName;
            if (!string.IsNullOrWhiteSpace(submission.Student?.StudentRoll))
            {
                var ext = Path.GetExtension(download.FileName);
                suggestedName = $"{submission.Student.StudentRoll}_submission{ext}";
            }

            return File(download.Stream, download.ContentType, suggestedName);
        }
        catch (FileNotFoundException)
        {
            return NotFound(ApiResponse<string>.FailResponse("Stored file could not be found."));
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                ApiResponse<string>.FailResponse("Failed to download submission file."));
        }
    }
}


