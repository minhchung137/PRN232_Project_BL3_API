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
                .Include(x => x.ClassGroups)
                .Include(x => x.SemesterSubjects)
                    .ThenInclude(ss => ss.Subject)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SemesterId == id);
        }

        public async Task<(IReadOnlyList<Semester> Items, int Total)> GetPagedWithDetailsAsync(
            Semester filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Semesters
                .Include(x => x.Exams)
                .Include(x => x.ClassGroups)
                .Include(x => x.SemesterSubjects)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.SemesterId > 0)
                    query = query.Where(x => x.SemesterId == filter.SemesterId);

                if (!string.IsNullOrWhiteSpace(filter.SemesterCode))
                    query = query.Where(x => x.SemesterCode.Contains(filter.SemesterCode));

                if (filter.IsActive.HasValue)
                    query = query.Where(x => x.IsActive == filter.IsActive);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<Semester> GetAllWithDetails()
        {
            return _dbContext.Semesters
                 .Include(x => x.Exams)
                 .Include(x => x.ClassGroups)
                 .Include(x => x.SemesterSubjects)
                     .ThenInclude(ss => ss.Subject)
                 .AsQueryable();
        }

        public async Task<bool> ExistsWithCodeAsync(string code, int excludeId)
        {
            return await _dbContext.Semesters
                .AnyAsync(s => s.SemesterCode == code && s.SemesterId != excludeId);
        }
        public async Task<bool> ExistsAsync(int semesterId)
        {
            return await _dbContext.Semesters.AnyAsync(s => s.SemesterId == semesterId);
        }

    }
}
