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
    public class GradeRepository : EntityRepository<Grade>, IGradeRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public GradeRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Grade> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Grades
                .Include(x => x.Submission)
                .Include(x => x.Marker)
                .Include(x => x.GradeDetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GradeId == id);
        }

        public async Task<(IReadOnlyList<Grade> Items, int Total)> GetPagedWithDetailsAsync(
     Grade filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Grades
                .Include(x => x.Submission)
                .Include(x => x.Marker)
                .Include(x => x.GradeDetails)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.GradeId > 0)
                    query = query.Where(x => x.GradeId == filter.GradeId);

                if (filter.SubmissionId.HasValue)
                    query = query.Where(x => x.SubmissionId == filter.SubmissionId);

                if (filter.MarkerId.HasValue)
                    query = query.Where(x => x.MarkerId == filter.MarkerId);

                if (filter.Q1.HasValue)
                    query = query.Where(x => x.Q1 == filter.Q1);

                if (filter.Q2.HasValue)
                    query = query.Where(x => x.Q2 == filter.Q2);

                if (filter.Q3.HasValue)
                    query = query.Where(x => x.Q3 == filter.Q3);

                if (filter.Q4.HasValue)
                    query = query.Where(x => x.Q4 == filter.Q4);

                if (filter.Q5.HasValue)
                    query = query.Where(x => x.Q5 == filter.Q5);

                if (filter.Q6.HasValue)
                    query = query.Where(x => x.Q6 == filter.Q6);

                if (filter.TotalScore.HasValue)
                    query = query.Where(x => x.TotalScore >= filter.TotalScore);
                if (!string.IsNullOrWhiteSpace(filter.Status))
                    query = query.Where(x => x.Status == filter.Status);

                if (filter.CreatedAt.HasValue)
                    query = query.Where(x => x.CreatedAt.Value.Date == filter.CreatedAt.Value.Date);

                if (filter.UpdatedAt.HasValue)
                    query = query.Where(x => x.UpdatedAt.Value.Date == filter.UpdatedAt.Value.Date);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }


        public IQueryable<Grade> GetAllWithDetails()
        {
            return _dbContext.Grades
                .Include(x => x.Submission)
                .Include(x => x.Marker)
                .Include(x => x.GradeDetails)
                .AsQueryable();
        }
        public async Task<bool> ExistsAsync(int gradeid)
        {
            return await _dbContext.Grades.AnyAsync(s => s.GradeId == gradeid);
        }
    }
}
