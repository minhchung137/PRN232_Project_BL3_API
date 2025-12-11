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
    public class GradedetailRepository : EntityRepository<GradeDetail>, IGradedetailRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public GradedetailRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<GradeDetail> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.GradeDetails
                .Include(x => x.Grade)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GradeDetailId == id);
        }

        public async Task<(IReadOnlyList<GradeDetail> Items, int Total)> GetPagedWithDetailsAsync(
            GradeDetail filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.GradeDetails
                .Include(x => x.Grade)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.GradeDetailId > 0)
                    query = query.Where(x => x.GradeDetailId == filter.GradeDetailId);

                if (filter.GradeId.HasValue)
                    query = query.Where(x => x.GradeId == filter.GradeId);

                if (!string.IsNullOrWhiteSpace(filter.QCode))
                    query = query.Where(x => x.QCode.Contains(filter.QCode));

                if (!string.IsNullOrWhiteSpace(filter.SubCode))
                    query = query.Where(x => x.SubCode.Contains(filter.SubCode));

                if (filter.Point.HasValue)
                    query = query.Where(x => x.Point >= filter.Point);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt) // sort theo ngày tạo mới nhất
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        public IQueryable<GradeDetail> GetAllWithDetails()
        {
            return _dbContext.GradeDetails
                 .Include(x => x.Grade)
                .AsQueryable();
        }
        public async Task<IEnumerable<GradeDetail>> GetByGradeIdAsync(int gradeId)
        {
            return await _dbContext.GradeDetails
                .Where(g => g.GradeId == gradeId)
                .ToListAsync();
        }
    }


}
