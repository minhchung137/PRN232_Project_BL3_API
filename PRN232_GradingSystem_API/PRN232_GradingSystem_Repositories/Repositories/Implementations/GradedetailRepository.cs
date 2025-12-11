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
    public class GradedetailRepository : EntityRepository<Gradedetail>, IGradedetailRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public GradedetailRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Gradedetail> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Gradedetails
                .Include(x => x.Grade)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Gradedetailid == id);
        }

        public async Task<(IReadOnlyList<Gradedetail> Items, int Total)> GetPagedWithDetailsAsync(
            Gradedetail filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Gradedetails
                .Include(x => x.Grade)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Gradedetailid > 0)
                    query = query.Where(x => x.Gradedetailid == filter.Gradedetailid);

                if (filter.Gradeid.HasValue)
                    query = query.Where(x => x.Gradeid == filter.Gradeid);

                if (!string.IsNullOrWhiteSpace(filter.Qcode))
                    query = query.Where(x => x.Qcode.Contains(filter.Qcode));

                if (!string.IsNullOrWhiteSpace(filter.Subcode))
                    query = query.Where(x => x.Subcode.Contains(filter.Subcode));

                if (filter.Point.HasValue)
                    query = query.Where(x => x.Point >= filter.Point);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.Createat) // sort theo ngày tạo mới nhất
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }
        public IQueryable<Gradedetail> GetAllWithDetails()
        {
            return _dbContext.Gradedetails
                 .Include(x => x.Grade)
                .AsQueryable();
        }
        public async Task<IEnumerable<Gradedetail>> GetByGradeIdAsync(int gradeId)
        {
            return await _dbContext.Gradedetails
                .Where(g => g.Gradeid == gradeId)
                .ToListAsync();
        }
    }


}
