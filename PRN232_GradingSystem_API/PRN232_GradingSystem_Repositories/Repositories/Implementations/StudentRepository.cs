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
    public class StudentRepository : EntityRepository<Student>, IStudentRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public StudentRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<Student> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Students
                .Include(x => x.GroupStudents)
                    .ThenInclude(gs => gs.Group)
                .Include(x => x.Submissions)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Studentid == id);
        }

        public async Task<(IReadOnlyList<Student> Items, int Total)> GetPagedWithDetailsAsync(
            Student filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Students
                .Include(x => x.GroupStudents)
                .Include(x => x.Submissions)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Studentid > 0)
                    query = query.Where(x => x.Studentid == filter.Studentid);

                if (!string.IsNullOrWhiteSpace(filter.Studentfullname))
                    query = query.Where(x => x.Studentfullname.Contains(filter.Studentfullname));

                if (!string.IsNullOrWhiteSpace(filter.Studentroll))
                    query = query.Where(x => x.Studentroll.Contains(filter.Studentroll));

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

        public IQueryable<Student> GetAllWithDetails()
        {
            return _dbContext.Students
                .Include(s => s.GroupStudents)
                    .ThenInclude(gs => gs.Group)
                .Include(s => s.Submissions)
                .AsQueryable();
        }
        public async Task<bool> ExistsByRollAsync(string studentRoll, int excludeId = 0)
        {
            return await _dbContext.Students
                .AnyAsync(s => s.Studentroll == studentRoll && (excludeId == 0 || s.Studentid != excludeId));
        }
        public async Task<bool> ExistsAsync(int studentId)
        {
            return await _dbContext.Students.AnyAsync(s => s.Studentid == studentId);
        }
    }
}
