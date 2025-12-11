using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IGroupRepository : IEntityRepository<ClassGroup>
    {
        Task<(IReadOnlyList<ClassGroup> Items, int Total)> GetPagedWithDetailsAsync(ClassGroup filter, int pageNumber, int pageSize);
        IQueryable<ClassGroup> GetAllWithDetails();
        Task<bool> ExistsWithNameAsync(string groupName, int semesterId, int excludeId = 0);
    }
}
