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
                .Include(x => x.MarkerNavigation)
                .Include(x => x.Gradedetails)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Gradeid == id);
        }

        public async Task<(IReadOnlyList<Grade> Items, int Total)> GetPagedWithDetailsAsync(
     Grade filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Grades
                .Include(x => x.Submission)
                .Include(x => x.MarkerNavigation)
                .Include(x => x.Gradedetails)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Gradeid > 0)
                    query = query.Where(x => x.Gradeid == filter.Gradeid);

                if (filter.Submissionid.HasValue)
                    query = query.Where(x => x.Submissionid == filter.Submissionid);

                if (filter.Marker.HasValue)
                    query = query.Where(x => x.Marker == filter.Marker);

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

                if (filter.Totalscore.HasValue)
                    query = query.Where(x => x.Totalscore >= filter.Totalscore);
                if (!string.IsNullOrWhiteSpace(filter.Status))
                    query = query.Where(x => x.Status == filter.Status);

                if (filter.Createat.HasValue)
                    query = query.Where(x => x.Createat.Value.Date == filter.Createat.Value.Date);

                if (filter.Updateat.HasValue)
                    query = query.Where(x => x.Updateat.Value.Date == filter.Updateat.Value.Date);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Createat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }


        public IQueryable<Grade> GetAllWithDetails()
        {
            return _dbContext.Grades
                .Include(x => x.Submission)
                .Include(x => x.MarkerNavigation)
                .Include(x => x.Gradedetails)
                .AsQueryable();
        }
        public async Task<bool> ExistsAsync(int gradeid)
        {
            return await _dbContext.Grades.AnyAsync(s => s.Gradeid == gradeid);
        }
    }
}
