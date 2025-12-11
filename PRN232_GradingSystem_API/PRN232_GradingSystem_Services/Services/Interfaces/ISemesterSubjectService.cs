using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface ISemesterSubjectService : ICrudService<SemesterSubjectBM>
    {
        Task<PagedResult<SemesterSubjectBM>> GetPagedFilteredAsync(SemesterSubjectBM filter, int pageNumber, int pageSize);
        IQueryable<SemesterSubjectBM> GetODataQueryable();
        Task<bool> DeleteAsync(int semesterId, int subjectId);
    }
}
