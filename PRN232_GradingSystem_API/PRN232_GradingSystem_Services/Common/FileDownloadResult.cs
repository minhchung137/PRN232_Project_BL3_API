using System;
using System.IO;
using System.Net.Mime;

namespace PRN232_GradingSystem_Services.Common;

public sealed class FileDownloadResult : IDisposable
{
    public Stream Stream { get; }
    public string ContentType { get; }
    public string FileName { get; }

    private readonly IDisposable _resource;

    public FileDownloadResult(Stream stream, string contentType, string fileName, IDisposable? resource = null)
    {
        Stream = stream ?? throw new ArgumentNullException(nameof(stream));
        ContentType = string.IsNullOrWhiteSpace(contentType) ? MediaTypeNames.Application.Octet : contentType;
        FileName = string.IsNullOrWhiteSpace(fileName) ? "downloaded-file" : fileName;
        _resource = resource ?? stream;
    }

    public void Dispose()
    {
        _resource.Dispose();
    }
}

