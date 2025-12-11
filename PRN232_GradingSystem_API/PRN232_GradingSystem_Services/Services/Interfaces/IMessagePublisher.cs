using System.Threading;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync(string message, CancellationToken cancellationToken = default);
}


