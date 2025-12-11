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
    public class GroupRepository : EntityRepository<Group>, IGroupRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public GroupRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Group> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Groups
                .Include(x => x.Semester)
                .Include(x => x.GroupStudents)
                .ThenInclude(gs => gs.Student)
                .Include(x => x.CreatebyNavigation)
                .Include(x => x.UpdatebyNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Groupid == id);
        }

        public async Task<(IReadOnlyList<Group> Items, int Total)> GetPagedWithDetailsAsync(
            Group filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Groups
                .Include(x => x.Semester)
                .Include(x => x.GroupStudents)
                .Include(x => x.CreatebyNavigation)
                .Include(x => x.UpdatebyNavigation)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Groupid > 0)
                    query = query.Where(x => x.Groupid == filter.Groupid);

                if (!string.IsNullOrWhiteSpace(filter.Groupname))
                    query = query.Where(x => x.Groupname.Contains(filter.Groupname));

                if (filter.Semesterid.HasValue)
                    query = query.Where(x => x.Semesterid == filter.Semesterid);

                if (filter.Createby.HasValue)
                    query = query.Where(x => x.Createby == filter.Createby);
                if (filter.Updateby.HasValue)
                    query = query.Where(x => x.Updateby == filter.Updateby);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.Createat) // sort theo ngày tạo mới nhất
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<Group> GetAllWithDetails()
        {
            return _dbContext.Groups
                .Include(x => x.Semester)
                .Include(x => x.GroupStudents)
                .Include(x => x.CreatebyNavigation)
                .Include(x => x.UpdatebyNavigation)
                .AsQueryable();
        }
        public async Task<bool> ExistsWithNameAsync(string groupName, int semesterId, int excludeId = 0)
        {
            return await _dbContext.Groups
                .AnyAsync(g =>
                    g.Groupname.ToLower() == groupName.ToLower() &&
                    g.Semesterid == semesterId &&
                    g.Groupid != excludeId);
        }

    }
}
