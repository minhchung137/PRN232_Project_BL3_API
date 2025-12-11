using Microsoft.EntityFrameworkCore;
using PRN232_GradingSystem_Repositories.DBContext;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.Repositories.Interfaces;
using ProductSaleApp.Repository.Repositories.Implementations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PRN232_GradingSystem_Repositories.Repositories.Implementations
{
    public class GroupRepository : EntityRepository<ClassGroup>, IGroupRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public GroupRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<ClassGroup> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.ClassGroups
                .Include(x => x.Semester)
                .Include(x => x.GroupStudents)
                .ThenInclude(gs => gs.Student)
                .Include(x => x.CreatedByNavigation)
                .Include(x => x.UpdatedByNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GroupId == id);
        }

        public async Task<(IReadOnlyList<ClassGroup> Items, int Total)> GetPagedWithDetailsAsync(
            ClassGroup filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.ClassGroups
                .Include(x => x.Semester)
                .Include(x => x.GroupStudents)
                .Include(x => x.CreatedByNavigation)
                .Include(x => x.UpdatedByNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.GroupId > 0)
                    query = query.Where(x => x.GroupId == filter.GroupId);

                if (!string.IsNullOrWhiteSpace(filter.GroupName))
                    query = query.Where(x => x.GroupName.Contains(filter.GroupName));

                if (filter.SemesterId.HasValue)
                    query = query.Where(x => x.SemesterId == filter.SemesterId);

                if (filter.CreatedBy.HasValue)
                    query = query.Where(x => x.CreatedBy == filter.CreatedBy);
                if (filter.UpdatedBy.HasValue)
                    query = query.Where(x => x.UpdatedBy == filter.UpdatedBy);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt) // sort theo ngày tạo mới nhất
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<ClassGroup> GetAllWithDetails()
        {
            return _dbContext.ClassGroups
                .Include(x => x.Semester)
                .Include(x => x.GroupStudents)
                .Include(x => x.CreatedByNavigation)
                .Include(x => x.UpdatedByNavigation)
                .AsQueryable();
        }
        public async Task<bool> ExistsWithNameAsync(string groupName, int semesterId, int excludeId = 0)
        {
            return await _dbContext.ClassGroups
                .AnyAsync(g =>
                    g.GroupName.ToLower() == groupName.ToLower() &&
                    g.SemesterId == semesterId &&
                    g.GroupId != excludeId);
        }

    }
}
