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
    public class SemesterSubjectRepository : EntityRepository<SemesterSubject>, ISemesterSubjectRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public SemesterSubjectRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<SemesterSubject> GetByIdWithDetailsAsync(int semesterId, int subjectId)
        {
            return _dbContext.SemesterSubjects
                .Include(x => x.Semester)
                .Include(x => x.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Semesterid == semesterId && x.Subjectid == subjectId);
        }
            
        public async Task<(IReadOnlyList<SemesterSubject> Items, int Total)> GetPagedWithDetailsAsync(
            SemesterSubject filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.SemesterSubjects
                .Include(x => x.Semester)
                .Include(x => x.Subject)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Semesterid > 0)
                    query = query.Where(x => x.Semesterid == filter.Semesterid);

                if (filter.Subjectid > 0)
                    query = query.Where(x => x.Subjectid == filter.Subjectid);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.Createat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        public IQueryable<SemesterSubject> GetAllWithDetails()
        {
            return _dbContext.SemesterSubjects
                .Include(x => x.Semester)
                .Include(x => x.Subject)
                .AsQueryable();
        }

        public async Task<bool> ExistsAsync(int semesterId, int subjectId)
        {
            return await _dbContext.SemesterSubjects
                .AnyAsync(ss => ss.Semesterid == semesterId && ss.Subjectid == subjectId);
        }
        public Task<SemesterSubject> GetByKeysAsync(int semesterId, int subjectId, bool trackChanges = false)
        {
            return _dbContext.SemesterSubjects
                .FirstOrDefaultAsync(ss => ss.Semesterid == semesterId && ss.Subjectid == subjectId);
        }

    }
}
