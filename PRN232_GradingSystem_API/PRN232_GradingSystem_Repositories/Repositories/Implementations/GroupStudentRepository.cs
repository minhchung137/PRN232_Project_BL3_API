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
    public class GroupStudentRepository : EntityRepository<GroupStudent>, IGroupStudentRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public GroupStudentRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<GroupStudent> GetByIdWithDetailsAsync(int groupId, int studentId)
        {
            return _dbContext.GroupStudents
                .Include(x => x.Group)
                .Include(x => x.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.GroupId == groupId && x.StudentId == studentId);
        }

        public async Task<(IReadOnlyList<GroupStudent> Items, int Total)> GetPagedWithDetailsAsync(
            GroupStudent filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.GroupStudents
                .Include(x => x.Group)
                .Include(x => x.Student)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.GroupId > 0)
                    query = query.Where(x => x.GroupId == filter.GroupId);

                if (filter.StudentId > 0)
                    query = query.Where(x => x.StudentId == filter.StudentId);
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public IQueryable<GroupStudent> GetAllWithDetails()
        {
            return _dbContext.GroupStudents
                .Include(x => x.Group)
                .Include(x => x.Student)
                .AsQueryable();
        }

        public async Task<GroupStudent?> GetByIdAsync(int groupId, int studentId, bool trackChanges = false)
        {
            var query = _dbContext.GroupStudents
                .Where(gs => gs.GroupId == groupId && gs.StudentId == studentId);

            if (!trackChanges)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        // ===== KIỂM TRA TỒN TẠI =====
        public async Task<bool> ExistsAsync(int groupId, int studentId)
        {
            return await _dbContext.GroupStudents
                .AnyAsync(gs => gs.GroupId == groupId && gs.StudentId == studentId);
        }
    }
}
