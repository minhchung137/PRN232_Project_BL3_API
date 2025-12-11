using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IGradedetailRepository : IEntityRepository<Gradedetail>
    {
        Task<(IReadOnlyList<Gradedetail> Items, int Total)> GetPagedWithDetailsAsync(Gradedetail filter, int pageNumber, int pageSize);
        IQueryable<Gradedetail> GetAllWithDetails();
        Task<IEnumerable<Gradedetail>> GetByGradeIdAsync(int gradeId);
    }
}
