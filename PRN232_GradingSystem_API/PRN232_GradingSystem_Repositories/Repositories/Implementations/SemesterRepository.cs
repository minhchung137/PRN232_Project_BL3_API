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
    public class SemesterRepository : EntityRepository<Semester>, ISemesterRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public SemesterRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Semester> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Semesters
                .Include(x => x.Exams)
                .Include(x => x.Groups)
                .Include(x => x.SemesterSubjects)
                    .ThenInclude(ss => ss.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Semesterid == id);
        }

        public async Task<(IReadOnlyList<Semester> Items, int Total)> GetPagedWithDetailsAsync(
            Semester filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Semesters
                .Include(x => x.Exams)
                .Include(x => x.Groups)
                .Include(x => x.SemesterSubjects)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Semesterid > 0)
                    query = query.Where(x => x.Semesterid == filter.Semesterid);

                if (!string.IsNullOrWhiteSpace(filter.Semestercode))
                    query = query.Where(x => x.Semestercode.Contains(filter.Semestercode));

                if (filter.Isactive.HasValue)
                    query = query.Where(x => x.Isactive == filter.Isactive);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Createat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<Semester> GetAllWithDetails()
        {
            return _dbContext.Semesters
                 .Include(x => x.Exams)
                 .Include(x => x.Groups)
                 .Include(x => x.SemesterSubjects)
                     .ThenInclude(ss => ss.Subject)
                 .AsQueryable();
        }

        public async Task<bool> ExistsWithCodeAsync(string code, int excludeId)
        {
            return await _dbContext.Semesters
                .AnyAsync(s => s.Semestercode == code && s.Semesterid != excludeId);
        }
        public async Task<bool> ExistsAsync(int semesterId)
        {
            return await _dbContext.Semesters.AnyAsync(s => s.Semesterid == semesterId);
        }

    }
}
