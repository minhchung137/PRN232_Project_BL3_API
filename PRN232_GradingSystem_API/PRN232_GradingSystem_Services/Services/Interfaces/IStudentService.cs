using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface IStudentService : ICrudService<StudentBM>
    {
        Task<PagedResult<StudentBM>> GetPagedFilteredAsync(StudentBM filter, int pageNumber, int pageSize);
        IQueryable<StudentBM> GetODataQueryable();
        Task<bool> DeactivateAsync(int id);
    }
}
