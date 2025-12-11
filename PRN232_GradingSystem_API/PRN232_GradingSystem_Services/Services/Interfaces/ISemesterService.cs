using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface ISemesterService : ICrudService<SemesterBM>
    {
        Task<PagedResult<SemesterBM>> GetPagedFilteredAsync(SemesterBM filter, int pageNumber, int pageSize);
        IQueryable<SemesterBM> GetODataQueryable();
        Task<SemesterBM?> DeactivateAsync(int id);
    }
}
