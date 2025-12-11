using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IGradedetailRepository : IEntityRepository<GradeDetail>
    {
        Task<(IReadOnlyList<GradeDetail> Items, int Total)> GetPagedWithDetailsAsync(GradeDetail filter, int pageNumber, int pageSize);
        IQueryable<GradeDetail> GetAllWithDetails();
        Task<IEnumerable<GradeDetail>> GetByGradeIdAsync(int gradeId);
    }
}
