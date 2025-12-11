using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IExamRepository : IEntityRepository<Exam>
    {
        Task<(IReadOnlyList<Exam> Items, int Total)> GetPagedWithDetailsAsync(Exam filter, int pageNumber, int pageSize);
        IQueryable<Exam> GetAllWithDetails();
        Task<bool> ExistsByNameAsync(string examName, int excludeId);
        Task<bool> ExistsAsync(int examid);
        Task<Exam> GetExamWithFullDataAsync(int examId);
    }
}
