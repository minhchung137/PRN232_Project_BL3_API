using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{

    public interface ICrudService<TBusinessModel>
    {
        Task<TBusinessModel> GetByIdAsync(int id, bool includeDetails = true);

        Task<PagedResult<TBusinessModel>> GetPagedAsync(int pageNumber, int pageSize);

        Task<TBusinessModel> CreateAsync(TBusinessModel model);

        Task<TBusinessModel> UpdateAsync(int id, TBusinessModel model);

        Task<bool> DeleteAsync(int id);
    }
}


