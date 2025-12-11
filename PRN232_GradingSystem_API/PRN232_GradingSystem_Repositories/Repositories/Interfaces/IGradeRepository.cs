using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IGradeRepository : IEntityRepository<Grade>
    {
        Task<(IReadOnlyList<Grade> Items, int Total)> GetPagedWithDetailsAsync(Grade filter, int pageNumber, int pageSize);
        IQueryable<Grade> GetAllWithDetails();
        Task<bool> ExistsAsync(int gradeid);
    }
}
