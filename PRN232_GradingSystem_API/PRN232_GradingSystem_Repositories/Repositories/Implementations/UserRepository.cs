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
    public class UserRepository : EntityRepository<User>, IUserRepository
    {
        private readonly PRN232_GradingSystem_APIContext _dbContext;

        public UserRepository(PRN232_GradingSystem_APIContext dbContext)
            : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public override Task<User> GetByIdWithDetailsAsync(int id)
        {
            return _dbContext.Users
                .Include(u => u.Grades)
                .Include(u => u.GroupCreatebyNavigations)
                .Include(u => u.GroupUpdatebyNavigations)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Userid == id);
        }

        public async Task<(IReadOnlyList<User> Items, int Total)> GetPagedWithDetailsAsync(
            User filter, int pageNumber, int pageSize)
        {
            var query = _dbContext.Users
                .Include(u => u.Grades)
                .Include(u => u.GroupCreatebyNavigations)
                .Include(u => u.GroupUpdatebyNavigations)
                .AsNoTracking()
                .AsQueryable();

            if (filter != null)
            {
                if (filter.Userid > 0)
                    query = query.Where(u => u.Userid == filter.Userid);

                if (!string.IsNullOrWhiteSpace(filter.Email))
                    query = query.Where(u => u.Email.Contains(filter.Email));

                if (!string.IsNullOrWhiteSpace(filter.Username))
                    query = query.Where(u => u.Username.Contains(filter.Username));

                if (!string.IsNullOrWhiteSpace(filter.Role))
                    query = query.Where(u => u.Role == filter.Role);

                if (filter.Isactive.HasValue)
                    query = query.Where(u => u.Isactive == filter.Isactive);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(u => u.Createat)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Refreshtoken == refreshToken);
        }

        public IQueryable<User> GetAllWithDetails()
        {
            return _dbContext.Users
                 .Include(u => u.Grades)
                 .Include(u => u.GroupCreatebyNavigations)
                 .Include(u => u.GroupUpdatebyNavigations)
                 .AsQueryable();
        }
        public async Task<bool> ExistsAsync(int userId)
        {
            return await _dbContext.Users.AnyAsync(u => u.Userid == userId);
        }
        public async Task<int?> GetUserIdByEmailAsync(string email)
        {
            return await _dbContext.Users
                .Where(u => u.Email == email)
                .Select(u => (int?)u.Userid)
                .FirstOrDefaultAsync();
        }
    }
}
