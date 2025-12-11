using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IGroupStudentRepository : IEntityRepository<GroupStudent>
    {
        Task<(IReadOnlyList<GroupStudent> Items, int Total)> GetPagedWithDetailsAsync(GroupStudent filter, int pageNumber, int pageSize);
        IQueryable<GroupStudent> GetAllWithDetails();
        Task<GroupStudent?> GetByIdAsync(int groupId, int studentId, bool trackChanges = false);
        Task<bool> ExistsAsync(int groupId, int studentId);

    }
}
