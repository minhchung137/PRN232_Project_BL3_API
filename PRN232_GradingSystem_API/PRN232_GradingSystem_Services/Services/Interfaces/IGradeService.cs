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
        Task<GradeBM> UpdateManualPartAsync(int gradeId, decimal? q7, decimal? q8, decimal? q9, decimal? q10, decimal? q11, decimal? q12, string note, int teacherId);
        Task<GradeBM> UpdateStatusAsync(int gradeId, string? status);

        Task<GradeBM> ModeratorReviewAsync(int gradeId, decimal? q1, decimal? q2, decimal? q3, decimal? q4, decimal? q5, decimal? q6, string note, int moderatorId);

        Task<bool> StudentRequestAppealAsync(int gradeId, string reason, int studentId);

        Task<PagedResult<GradeBM>> GetPendingAppealsAsync(int pageNumber, int pageSize);
    }
}
