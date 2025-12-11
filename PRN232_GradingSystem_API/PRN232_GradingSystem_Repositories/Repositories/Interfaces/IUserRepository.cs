using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IUserRepository : IEntityRepository<User>
    {
        Task<(IReadOnlyList<User> Items, int Total)> GetPagedWithDetailsAsync(User filter, int pageNumber, int pageSize);

        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByRefreshTokenAsync(string refreshToken);
        IQueryable<User> GetAllWithDetails();

        Task<bool> ExistsAsync(int userId);
        Task<int?> GetUserIdByEmailAsync(string email);
    }
}
