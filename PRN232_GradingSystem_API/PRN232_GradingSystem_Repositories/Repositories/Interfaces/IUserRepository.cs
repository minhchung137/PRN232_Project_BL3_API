using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IUserRepository : IEntityRepository<AppUser>
    {
        Task<(IReadOnlyList<AppUser> Items, int Total)> GetPagedWithDetailsAsync(AppUser filter, int pageNumber, int pageSize);

        Task<AppUser> GetByUsernameAsync(string username);
        Task<AppUser> GetByEmailAsync(string email);
        Task<AppUser> GetByRefreshTokenAsync(string refreshToken);
        IQueryable<AppUser> GetAllWithDetails();

        Task<bool> ExistsAsync(int userId);
        Task<int?> GetUserIdByEmailAsync(string email);
    }
}
