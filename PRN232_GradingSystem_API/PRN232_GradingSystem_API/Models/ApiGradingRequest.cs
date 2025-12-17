using Microsoft.AspNetCore.Http;

namespace PRN232_GradingSystem_API.Models;

public class ApiGradingRequest
{
    public IFormFile File { get; set; } = null!;
    public string EntityName { get; set; } = string.Empty;
}

public class ApiGradingFromUrlRequest
{
    public string FileUrl { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
}





