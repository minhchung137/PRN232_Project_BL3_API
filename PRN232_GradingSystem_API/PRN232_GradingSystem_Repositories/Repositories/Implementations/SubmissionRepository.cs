using Microsoft.EntityFrameworkCore;
using PRN232_GradingSystem_Repositories.DBContext;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.Repositories.Interfaces;
using ProductSaleApp.Repository.Repositories.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Implementations
{
    public class SubmissionRepository : EntityRepository<Submission>, ISubmissionRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public SubmissionRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Submission> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Submissions
                .Include(s => s.Exam)
                .Include(s => s.Student)
                .Include(s => s.Grades)
                    .ThenInclude(g => g.GradeDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SubmissionId == id);
        }

        public async Task<(IReadOnlyList<Submission> Items, int Total)> GetPagedWithDetailsAsync(
            Submission filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Submissions
                .Include(s => s.Exam)
                .Include(s => s.Student)
                .Include(s => s.Grades)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.SubmissionId > 0)
                    query = query.Where(s => s.SubmissionId == filter.SubmissionId);

                if (filter.StudentId.HasValue)
                    query = query.Where(s => s.StudentId == filter.StudentId);

                if (filter.ExamId.HasValue)
                    query = query.Where(s => s.ExamId == filter.ExamId);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<Submission> GetAllWithDetails()
        {
            return _dbContext.Submissions
                 .Include(s => s.Exam)
                 .Include(s => s.Student)
                 .Include(s => s.Grades)
                     .ThenInclude(g => g.GradeDetails)
                 .AsQueryable();
        }
        public async Task<bool> ExistsAsync(int submissionId)
        {
            return await _dbContext.Submissions.AnyAsync(s => s.SubmissionId == submissionId);
        }
    }
}
