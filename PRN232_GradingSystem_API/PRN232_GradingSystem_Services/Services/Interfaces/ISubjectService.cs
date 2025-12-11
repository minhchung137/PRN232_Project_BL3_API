using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface ISubjectService : ICrudService<SubjectBM>
    {
        Task<PagedResult<SubjectBM>> GetPagedFilteredAsync(SubjectBM filter, int pageNumber, int pageSize);
        IQueryable<SubjectBM> GetODataQueryable();
    
    }
}
