using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface IUserService : ICrudService<UserBM>
    {
        Task<PagedResult<UserBM>> GetPagedFilteredAsync(UserBM filter, int pageNumber, int pageSize);

        Task<ApiResponse<AuthResultModel>> RegisterAsync(string username, string email, string password);
        Task<ApiResponse<AuthResultModel>> LoginAsync(string usernameOrEmail, string password);
        Task<ApiResponse<AuthResultModel>> RefreshTokenAsync(string refreshToken);
        Task<ApiResponse<object>> LogoutAsync(string refreshToken);
        IQueryable<UserBM> GetODataQueryable();

    }
}
