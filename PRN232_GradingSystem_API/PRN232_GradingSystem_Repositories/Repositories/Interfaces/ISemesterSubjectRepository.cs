using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface ISemesterSubjectRepository : IEntityRepository<SemesterSubject>
    {
        Task<(IReadOnlyList<SemesterSubject> Items, int Total)> GetPagedWithDetailsAsync(SemesterSubject filter, int pageNumber, int pageSize);
        IQueryable<SemesterSubject> GetAllWithDetails();
        Task<bool> ExistsAsync(int semesterId, int subjectId);
        Task<SemesterSubject> GetByKeysAsync(int semesterId, int subjectId, bool trackChanges = false);
    }
}
