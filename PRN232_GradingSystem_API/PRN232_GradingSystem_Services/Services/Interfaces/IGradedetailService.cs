using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface IGradedetailService : ICrudService<GradedetailBM>
    {
        Task<PagedResult<GradedetailBM>> GetPagedFilteredAsync(GradedetailBM filter, int pageNumber, int pageSize);

        IQueryable<GradedetailBM> GetODataQueryable();
        Task UpdateManyAsync(GradedetailUpdateRequestListBM request);
    }
}
