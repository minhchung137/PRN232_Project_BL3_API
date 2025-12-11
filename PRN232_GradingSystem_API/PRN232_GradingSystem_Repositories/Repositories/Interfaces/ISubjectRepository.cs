using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface ISubjectRepository : IEntityRepository<Subject>
    {
        Task<(IReadOnlyList<Subject> Items, int Total)> GetPagedWithDetailsAsync(Subject filter, int pageNumber, int pageSize);
        IQueryable<Subject> GetAllWithDetails();
        Task<bool> ExistsByNameAsync(string name, int excludeId = 0);
        Task<bool> ExistsAsync(int semesterId);
    }
}
