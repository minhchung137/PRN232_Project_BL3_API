using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface ISemesterRepository : IEntityRepository<Semester>
    {
        Task<(IReadOnlyList<Semester> Items, int Total)> GetPagedWithDetailsAsync(Semester filter, int pageNumber, int pageSize);
        IQueryable<Semester> GetAllWithDetails();
        Task<bool> ExistsWithCodeAsync(string code, int excludeId);
        Task<bool> ExistsAsync(int semesterId);

    }
}
