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
    public class SubjectRepository : EntityRepository<Subject>, ISubjectRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public SubjectRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Subject> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Subjects
                .Include(s => s.Exams)
                .Include(s => s.SemesterSubjects)
                    .ThenInclude(ss => ss.Semester)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Subjectid == id);
        }

        public async Task<(IReadOnlyList<Subject> Items, int Total)> GetPagedWithDetailsAsync(
            Subject filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Subjects
                .Include(s => s.Exams)
                .Include(s => s.SemesterSubjects)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Subjectid > 0)
                    query = query.Where(s => s.Subjectid == filter.Subjectid);

                if (!string.IsNullOrWhiteSpace(filter.Subjectname))
                    query = query.Where(s => s.Subjectname.Contains(filter.Subjectname));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(s => s.Createat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<Subject> GetAllWithDetails()
        {
            return _dbContext.Subjects
                .Include(s => s.Exams)
                .Include(s => s.SemesterSubjects)
                    .ThenInclude(ss => ss.Semester)
                .AsQueryable();
        }
        public async Task<bool> ExistsByNameAsync(string name, int excludeId = 0)
        {
            return await _dbContext.Subjects
                .AnyAsync(s => s.Subjectid != excludeId && s.Subjectname == name);
        }
        public async Task<bool> ExistsAsync(int subjectId)
        {
            return await _dbContext.Subjects.AnyAsync(s => s.Subjectid == subjectId);
        }
    }
}
