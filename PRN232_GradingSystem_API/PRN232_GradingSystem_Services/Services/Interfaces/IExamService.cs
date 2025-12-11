using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface IExamService : ICrudService<ExamBM>
    {
        Task<PagedResult<ExamBM>> GetPagedFilteredAsync(ExamBM filter, int pageNumber, int pageSize);
        IQueryable<ExamBM> GetODataQueryable();
        Task<ExamBM?> GetExamByIdAsync(int examId);
    }
}
