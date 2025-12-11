using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface ISubmissionService : ICrudService<SubmissionBM>
    {
        Task<PagedResult<SubmissionBM>> GetPagedFilteredAsync(SubmissionBM filter, int pageNumber, int pageSize);
        IQueryable<SubmissionBM> GetODataQueryable();
        Task<SubmissionBM> FindOrCreateSubmissionAsync(int examId, int studentId, string fileUrl, string studentRoll = null);
        Task<SubmissionBM> UpdateSubmissionAsync(int id, SubmissionBM model);
        Task<SubmissionBM> CreateSubmissionAsync(SubmissionBM model);
    }
}
