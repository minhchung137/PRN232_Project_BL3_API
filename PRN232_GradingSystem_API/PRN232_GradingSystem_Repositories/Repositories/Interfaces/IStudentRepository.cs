using PRN232_GradingSystem_Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Repositories.Repositories.Interfaces
{
    public interface IStudentRepository : IEntityRepository<Student>
    {
        Task<(IReadOnlyList<Student> Items, int Total)> GetPagedWithDetailsAsync(Student filter, int pageNumber, int pageSize);
        IQueryable<Student> GetAllWithDetails();
        Task<bool> ExistsByRollAsync(string studentRoll, int excludeId = 0);
        Task<bool> ExistsAsync(int studentId);
    }
}
