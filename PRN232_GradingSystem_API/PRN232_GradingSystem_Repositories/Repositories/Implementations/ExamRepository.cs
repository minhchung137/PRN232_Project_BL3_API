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
    public class ExamRepository : EntityRepository<Exam>, IExamRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public ExamRepository(PRN232_GradingSystem_APIContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Exam> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Exams
                .Include(x => x.Semester)
                .Include(x => x.Subject)
                .Include(x => x.Submissions)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ExamId == id);
        }

        public async Task<(IReadOnlyList<Exam> Items, int Total)> GetPagedWithDetailsAsync(
            Exam filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Exams
                .Include(x => x.Semester)
                .Include(x => x.Subject)
                .Include(x => x.Submissions)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.ExamId > 0)
                    query = query.Where(x => x.ExamId == filter.ExamId);

                if (!string.IsNullOrWhiteSpace(filter.ExamName))
                    query = query.Where(x => x.ExamName.Contains(filter.ExamName));

                if (filter.SemesterId.HasValue)
                    query = query.Where(x => x.SemesterId == filter.SemesterId);

                if (filter.SubjectId.HasValue)
                    query = query.Where(x => x.SubjectId == filter.SubjectId);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt) 
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<Exam> GetAllWithDetails()
        {
            return _dbContext.Exams
                .Include(e => e.Semester)
                .Include(e => e.Subject)
                .AsQueryable();
        }
        public async Task<bool> ExistsByNameAsync(string examName, int excludeId)
        {
            return await _dbContext.Exams.AnyAsync(e =>
                e.ExamName.ToLower() == examName.ToLower() && e.ExamId != excludeId);
        }
        public async Task<bool> ExistsAsync(int examid)
        {
            return await _dbContext.Exams.AnyAsync(s => s.ExamId == examid);
        }
        public async Task<Exam> GetExamWithFullDataAsync(int examId)
        {
            return await _dbContext.Exams
                .Include(e => e.Submissions)
                    .ThenInclude(s => s.Student)
                        .ThenInclude(st => st.GroupStudents)
                            .ThenInclude(gs => gs.Group)
                .Include(e => e.Submissions)
                    .ThenInclude(s => s.Grades)
                        .ThenInclude(g => g.GradeDetails)
                .Include(e => e.Submissions)
                    .ThenInclude(s => s.Grades)
                        .ThenInclude(g => g.Marker)
                .FirstOrDefaultAsync(e => e.ExamId == examId);
        }

    }
}
