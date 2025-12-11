using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Interfaces
{
    public interface IGroupStudentService : ICrudService<GroupStudentBM>
    {
        Task<PagedResult<GroupStudentBM>> GetPagedFilteredAsync(GroupStudentBM filter, int pageNumber, int pageSize);
        IQueryable<GroupStudentBM> GetODataQueryable();
        Task<bool> DeleteAsync(int groupId, int studentId);
        Task<GroupStudentBM> CreateAsync(GroupStudentBM model);
        //Task<GroupStudentBM> UpdateAsync(int groupId, int studentId, GroupStudentBM model);
        Task<GroupStudentBM?> GetByIdAsync(int groupId, int studentId);
    }
}
