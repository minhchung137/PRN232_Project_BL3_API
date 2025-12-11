using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using PRN232_GradingSystem_Services.Services.Interfaces;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using System.IO.Compression;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using System.Text.Json;
using PRN232_GradingSystem_Services.Common;

namespace PRN232_GradingSystem_Services.Services.Implementations;

public class BackblazeStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _endpoint;
    private readonly IMessagePublisher _publisher;
    private readonly ISubmissionService _submissionService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IExamService _examService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BackblazeStorageService(IConfiguration configuration, IMessagePublisher publisher, ISubmissionService submissionService, IUnitOfWork unitOfWork, IExamService examService, IHttpContextAccessor httpContextAccessor)
    {
        _endpoint = configuration["Backblaze:Endpoint"] ?? "";
        var keyId = configuration["Backblaze:KeyId"] ?? "";
        var applicationKey = configuration["Backblaze:ApplicationKey"] ?? "";
        _bucketName = configuration["Backblaze:BucketName"] ?? "";
        _publisher = publisher;
        _submissionService = submissionService;
        _unitOfWork = unitOfWork;
        _examService = examService;
        _httpContextAccessor = httpContextAccessor;

        var credentials = new BasicAWSCredentials(keyId, applicationKey);
        var config = new AmazonS3Config
        {
            ServiceURL = _endpoint,
            ForcePathStyle = true,
            SignatureVersion = "4"
        };
        _s3Client = new AmazonS3Client(credentials, config);
    }

    public async Task<IReadOnlyList<string>> UploadArchiveAsync(Stream contentStream, string fileName, string contentType, string prefix, int examId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("fileName is required", nameof(fileName));
        }

        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        if (extension != ".zip" && extension != ".rar")
        {
            throw new InvalidOperationException("Only .zip or .rar files are allowed.");
        }

        var prefixSegment = string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix.TrimEnd('/') + "/";
        const int defaultExamId = 1;
        var resolvedExamId = examId > 0 ? examId : defaultExamId;

        // Buffer archive to MemoryStream for multiple reads during extraction
        using var archiveBuffer = new MemoryStream();
        await contentStream.CopyToAsync(archiveBuffer, cancellationToken);
        archiveBuffer.Position = 0;

        var uploadedUrls = new List<string>();

        if (extension == ".zip")
        {
            using var zip = new ZipArchive(archiveBuffer, ZipArchiveMode.Read, leaveOpen: true);
            var topName = GetTopLevelNameFromZip(zip) ?? Path.GetFileNameWithoutExtension(fileName);
            foreach (var entry in zip.Entries)
            {
                if (string.IsNullOrEmpty(entry.Name)) continue; // skip directories
                if (!IsSolutionZip(entry.Name)) continue; // only solution.zip
                await using var entryStream = entry.Open();
                var relative = GetRelativePath(entry.FullName, topName);
                var entryKey = $"{prefixSegment}{SanitizeSegment(topName)}/{SanitizePath(relative)}";
                var url = await UploadStreamAsync(entryStream, entryKey, "application/zip", cancellationToken);
                
                // Extract student code and solution
                var studentCode = ExtractStudentCode(relative);
                var solution = ExtractSolution(relative);
                var submission = await FindOrCreateSubmissionAsync(studentCode, entryKey, resolvedExamId, solution, cancellationToken);
                
                var exam = await _examService.GetExamByIdAsync(resolvedExamId);
                var examCode = exam?.Examname ?? "PE_PRN222_SU25_332278"; // fallback if exam not found
                
                // Get ExaminerCode from current user email
                var examinerCode = GetCurrentUserEmail();
                
                // Publish message to RabbitMQ
                if (submission.Submissionid > 0)
                {
                    try
                    {
                        await PublishUploadMessageAsync(submission.Submissionid, entryKey, examCode, studentCode, examinerCode, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't fail the upload
                        // In production, use proper logging (ILogger)
                        System.Diagnostics.Debug.WriteLine($"Failed to publish message for SubmissionId {submission.Submissionid}: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: SubmissionId is invalid ({submission.Submissionid}), skipping message publish");
                }
                
                uploadedUrls.Add(url);
            }
        }
        else // .rar
        {
            archiveBuffer.Position = 0;
            using var rar = RarArchive.Open(archiveBuffer);
            var topName = GetTopLevelNameFromRar(rar) ?? Path.GetFileNameWithoutExtension(fileName);
            foreach (var entry in rar.Entries.Where(e => !e.IsDirectory))
            {
                if (!IsSolutionZip(Path.GetFileName(entry.Key))) continue; // only solution.zip
                await using var entryStream = new MemoryStream();
                entry.WriteTo(entryStream);
                entryStream.Position = 0;
                var relative = GetRelativePath(entry.Key, topName);
                var entryKey = $"{prefixSegment}{SanitizeSegment(topName)}/{SanitizePath(relative)}";
                var url = await UploadStreamAsync(entryStream, entryKey, "application/zip", cancellationToken);
                
                // Extract student code and solution
                var studentCode = ExtractStudentCode(relative);
                var solution = ExtractSolution(relative);
                var submission = await FindOrCreateSubmissionAsync(studentCode, entryKey, resolvedExamId, solution, cancellationToken);
                
                var exam = await _examService.GetExamByIdAsync(resolvedExamId);
                var examCode = exam?.Examname ?? "PE_PRN222_SU25_332278"; // fallback if exam not found
                
                // Get ExaminerCode from current user email
                var examinerCode = GetCurrentUserEmail();
                
                // Publish message to RabbitMQ
                if (submission.Submissionid > 0)
                {
                    try
                    {
                        await PublishUploadMessageAsync(submission.Submissionid, entryKey, examCode, studentCode, examinerCode, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't fail the upload
                        // In production, use proper logging (ILogger)
                        System.Diagnostics.Debug.WriteLine($"Failed to publish message for SubmissionId {submission.Submissionid}: {ex.Message}");
                        System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Warning: SubmissionId is invalid ({submission.Submissionid}), skipping message publish");
                }
                
                uploadedUrls.Add(url);
            }
        }

        return uploadedUrls;
    }

    public async Task<FileDownloadResult> DownloadFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileLocation))
        {
            throw new ArgumentException("fileLocation is required", nameof(fileLocation));
        }

        var key = ExtractKey(fileLocation);

        try
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, key, cancellationToken);
            var contentType = response.Headers?.ContentType ?? MediaTypeNames.Application.Octet;
            var fileName = Path.GetFileName(key);
            return new FileDownloadResult(response.ResponseStream, contentType, fileName, response);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new FileNotFoundException($"File '{key}' not found in storage.", key, ex);
        }
    }

    private async Task<string> UploadStreamAsync(Stream stream, string key, string contentType, CancellationToken cancellationToken)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = stream,
            ContentType = string.IsNullOrWhiteSpace(contentType) ? MediaTypeNames.Application.Octet : contentType,
            AutoCloseStream = false
        };
        await _s3Client.PutObjectAsync(putRequest, cancellationToken);
        return $"{_endpoint.TrimEnd('/')}/{_bucketName}/{Uri.EscapeDataString(key)}";
    }

    private static string? GetTopLevelNameFromZip(ZipArchive zip)
    {
        var first = zip.Entries.FirstOrDefault();
        if (first == null) return null;
        var fullName = first.FullName.Replace('\\', '/');
        var idx = fullName.IndexOf('/');
        return idx > 0 ? fullName.Substring(0, idx) : Path.GetFileNameWithoutExtension(first.Name);
    }

    private static string GetRelativePath(string fullName, string topName)
    {
        var normalized = (fullName ?? string.Empty).Replace('\\', '/');
        var tn = SanitizeSegment(topName);
        if (normalized.StartsWith(tn + "/", StringComparison.OrdinalIgnoreCase))
        {
            return normalized.Substring(tn.Length + 1);
        }
        // If it starts with raw topName (not sanitized), strip that too
        if (normalized.StartsWith(topName + "/", StringComparison.OrdinalIgnoreCase))
        {
            return normalized.Substring(topName.Length + 1);
        }
        return normalized;
    }

    private Task PublishUploadMessageAsync(int submissionId, string entryKey, string examCode, string studentId, string examinerCode, CancellationToken cancellationToken)
    {
        var payload = JsonSerializer.Serialize(new 
        { 
            SubmissionId = submissionId.ToString(), 
            FileUrl = entryKey,
            ExamCode = examCode,
            StudentId = studentId,
            ExaminerCode = examinerCode
        });
        return _publisher.PublishAsync(payload, cancellationToken);
    }

    private string GetCurrentUserEmail()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            // Try to get email from claims (JWT typically has email claim)
            var emailClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Email || c.Type == "email");
            if (emailClaim != null)
            {
                return emailClaim.Value;
            }
            
            // Fallback to NameIdentifier or Name
            var nameClaim = httpContext.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Name || c.Type == "name");
            if (nameClaim != null)
            {
                return nameClaim.Value;
            }
        }
        return string.Empty; // Return empty if no user found
    }

    private string ExtractStudentCode(string relativePath)
    {
        // Extract folder name from path like "anhdlse181818/0/solution.zip"
        // Get first folder segment (student code)
        var parts = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return string.Empty;
        
        var folderName = parts[0];
        // Get last 8 characters
        return folderName.Length >= 8 ? folderName.Substring(folderName.Length - 8) : folderName;
    }

    private string ExtractSolution(string relativePath)
    {
        // Extract full folder name from path like "anhdlse181818/0/solution.zip"
        // Get first folder segment (full solution name)
        var parts = relativePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0) return string.Empty;
        
        // Return the full folder name (e.g., "anhdlse181818")
        return parts[0];
    }

    private async Task<PRN232_GradingSystem_Services.BusinessModel.SubmissionBM> FindOrCreateSubmissionAsync(string studentCode, string fileUrl, int examId, string solution, CancellationToken cancellationToken)
    {
        // Find student by Studentroll (student code) - case insensitive
        var studentCodeLower = studentCode?.ToLowerInvariant() ?? string.Empty;
        var student = _unitOfWork.StudentRepository.GetAllWithDetails()
            .FirstOrDefault(s => s.Studentroll != null && s.Studentroll.ToLower() == studentCodeLower);
        
        if (student == null)
        {
            throw new InvalidOperationException($"Student with code '{studentCode}' not found.");
        }

        // Use solution (full folder name like "anhdlse181818") for Solution field
        var submission = await _submissionService.FindOrCreateSubmissionAsync(examId, student.Studentid, fileUrl, solution);
        return submission;
    }

    private static string? GetTopLevelNameFromRar(RarArchive rar)
    {
        var first = rar.Entries.FirstOrDefault();
        if (first == null) return null;
        var key = first.Key.Replace('\\', '/');
        var idx = key.IndexOf('/');
        return idx > 0 ? key.Substring(0, idx) : Path.GetFileNameWithoutExtension(first.Key);
    }

    private static string SanitizePath(string path)
    {
        var normalized = path.Replace('\\', '/');
        return string.Join('/', normalized.Split('/', StringSplitOptions.RemoveEmptyEntries).Select(SanitizeSegment));
    }

    private static string SanitizeSegment(string segment)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
        {
            segment = segment.Replace(c, '_');
        }
        return segment;
    }

    private static string GetContentTypeFromExtension(string ext)
    {
        ext = (ext ?? string.Empty).ToLowerInvariant();
        return ext switch
        {
            ".txt" => "text/plain",
            ".md" => "text/markdown",
            ".json" => "application/json",
            ".xml" => "application/xml",
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            _ => MediaTypeNames.Application.Octet
        };
    }

    private static bool IsSolutionZip(string name)
    {
        return string.Equals(name, "solution.zip", StringComparison.OrdinalIgnoreCase);
    }

    private string ExtractKey(string fileLocation)
    {
        var trimmed = fileLocation.Trim();

        if (Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
        {
            var path = Uri.UnescapeDataString(uri.AbsolutePath.TrimStart('/'));
            if (path.StartsWith($"{_bucketName}/", StringComparison.OrdinalIgnoreCase))
            {
                path = path.Substring(_bucketName.Length + 1);
            }
            return path;
        }

        return trimmed.TrimStart('/');
    }

}


