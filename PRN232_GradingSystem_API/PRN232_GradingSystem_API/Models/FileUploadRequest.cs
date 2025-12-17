using Microsoft.AspNetCore.Http;

namespace PRN232_GradingSystem_API.Models;

public class FileUploadRequest
{
    public IFormFile? File { get; set; }
    public string? Prefix { get; set; }
    public int? ExamId { get; set; }
    public string? EntityName { get; set; } // Entity name for API grading (e.g., "Student", "Product")
}


