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
                    .ThenInclude(g => g.Gradedetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Submissionid == id);
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
                if (filter.Submissionid > 0)
                    query = query.Where(s => s.Submissionid == filter.Submissionid);

                if (filter.Studentid.HasValue)
                    query = query.Where(s => s.Studentid == filter.Studentid);

                if (filter.Examid.HasValue)
                    query = query.Where(s => s.Examid == filter.Examid);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.Createat)
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
                     .ThenInclude(g => g.Gradedetails)
                 .AsQueryable();
        }
        public async Task<bool> ExistsAsync(int submissionId)
        {
            return await _dbContext.Submissions.AnyAsync(s => s.Submissionid == submissionId);
        }
    }
}
