using PRN232_GradingSystem_Repositories.Models;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendGradeCreatedNotificationAsync(Grade grade);
    }
}

