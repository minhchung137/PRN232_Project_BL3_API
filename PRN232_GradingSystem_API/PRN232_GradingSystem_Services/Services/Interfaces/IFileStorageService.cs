using System.IO;
using System.Threading;
using System.Threading.Tasks;
using PRN232_GradingSystem_Services.Common;

namespace PRN232_GradingSystem_Services.Services.Interfaces;

public interface IFileStorageService
{
    Task<IReadOnlyList<string>> UploadArchiveAsync(Stream contentStream, string fileName, string contentType, string prefix, int examId, CancellationToken cancellationToken = default);
    Task<FileDownloadResult> DownloadFileAsync(string fileLocation, CancellationToken cancellationToken = default);
}

