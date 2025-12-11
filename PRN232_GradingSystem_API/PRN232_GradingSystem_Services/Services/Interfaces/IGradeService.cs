using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface IGradeService : ICrudService<GradeBM>
    {
        Task<PagedResult<GradeBM>> GetPagedFilteredAsync(GradeBM filter, int pageNumber, int pageSize);

        IQueryable<GradeBM> GetODataQueryable();
        Task<GradeBM> CreateGradeWithDetailsAsync(GradeWithDetailsRequestBM model);
        Task<GradeBM> UpdateStatusAsync(int gradeId, string? status);
    }
}
