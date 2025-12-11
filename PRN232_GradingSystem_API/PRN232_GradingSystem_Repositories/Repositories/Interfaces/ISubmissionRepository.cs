using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface ISubmissionRepository : IEntityRepository<Submission>
    {
        Task<(IReadOnlyList<Submission> Items, int Total)> GetPagedWithDetailsAsync(Submission filter, int pageNumber, int pageSize);
        IQueryable<Submission> GetAllWithDetails();
        Task<bool> ExistsAsync(int submissionId);
    }
}
