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
    public class UserRepository : EntityRepository<AppUser>, IUserRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public UserRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<AppUser> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.AppUsers
                .Include(u => u.Grades)
                .Include(u => u.ClassGroupCreatedByNavigations)
                .Include(u => u.ClassGroupUpdatedByNavigations)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<(IReadOnlyList<AppUser> Items, int Total)> GetPagedWithDetailsAsync(
            AppUser filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.AppUsers
                .Include(u => u.Grades)
                .Include(u => u.ClassGroupCreatedByNavigations)
                .Include(u => u.ClassGroupUpdatedByNavigations)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.UserId > 0)
                    query = query.Where(u => u.UserId == filter.UserId);

                if (!string.IsNullOrWhiteSpace(filter.Email))
                    query = query.Where(u => u.Email.Contains(filter.Email));

                if (!string.IsNullOrWhiteSpace(filter.Username))
                    query = query.Where(u => u.Username.Contains(filter.Username));

                if (!string.IsNullOrWhiteSpace(filter.Role))
                    query = query.Where(u => u.Role == filter.Role);

                if (filter.IsActive.HasValue)
                    query = query.Where(u => u.IsActive == filter.IsActive);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(u => u.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<AppUser> GetByUsernameAsync(string username)
        {
            return await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<AppUser> GetByEmailAsync(string email)
        {
            return await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<AppUser> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbContext.AppUsers.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

        public IQueryable<AppUser> GetAllWithDetails()
        {
            return _dbContext.AppUsers
                 .Include(u => u.Grades)
                 .Include(u => u.ClassGroupCreatedByNavigations)
                 .Include(u => u.ClassGroupUpdatedByNavigations)
                 .AsQueryable();
        }
        public async Task<bool> ExistsAsync(int userId)
        {
            return await _dbContext.AppUsers.AnyAsync(u => u.UserId == userId);
        }
        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            return await _dbContext.AppUsers
                .Where(u => u.Email == email)
                .Select(u => (int?)u.UserId)
                .FirstOrDefaultAsync();
        }
    }
}
