using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class GroupStudentService : CrudService<GroupStudent, GroupStudentBM>, IGroupStudentService
    {
        private readonly IMapper _mapper;

        public GroupStudentService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<GroupStudent>
            GetRepository() => UnitOfWork.GroupStudentRepository;

        public async Task<PagedResult<GroupStudentBM>> GetPagedFilteredAsync(GroupStudentBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.GroupStudentRepository;

            var repositoryFilter = new GroupStudent
            {
                GroupId = filter?.Groupid ?? 0,
                StudentId = filter?.Studentid ?? 0
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<GroupStudentBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<GroupStudentBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }
        public IQueryable<GroupStudentBM> GetODataQueryable()
        {
            var repo = UnitOfWork.GroupStudentRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<GroupStudentBM>(query);
        }

        // ===== CREATE =====
        public override async Task<GroupStudentBM> CreateAsync(GroupStudentBM model)
        {
            var repo = UnitOfWork.GroupStudentRepository;
            var groupRepo = UnitOfWork.GroupRepository;
            var studentRepo = UnitOfWork.StudentRepository;

            // ===== VALIDATION =====
            var groupExists = await groupRepo.GetByIdAsync(model.Groupid);
            if (groupExists == null)
                throw new NotFoundException($"Group with ID '{model.Groupid}' does not exist.");

            var studentExists = await studentRepo.GetByIdAsync(model.Studentid);
            if (studentExists == null)
                throw new NotFoundException($"Student with ID '{model.Studentid}' does not exist.");

            // Kiểm tra trùng khóa chính (GroupId + StudentId)
            var alreadyExists = await repo.ExistsAsync(model.Groupid, model.Studentid);
            if (alreadyExists)
                throw new ConflictException($"Student with ID '{model.Studentid}' already belongs to Group '{model.Groupid}'.");

            // ===== MAP & SAVE =====
            var entity = _mapper.Map<GroupStudent>(model);
            entity.CreatedAt = DateTime.UtcNow;

            await repo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            // ===== MAP BACK =====
            return _mapper.Map<GroupStudentBM>(entity);
        }

        // ===== UPDATE =====
        //public async Task<GroupStudentBM> UpdateAsync(int groupId, int studentId, GroupStudentBM model)
        //{
        //    var repo = UnitOfWork.GroupStudentRepository;

        //    // ===== LẤY DỮ LIỆU GỐC =====
        //    var existing = await repo.GetByIdAsync(groupId, studentId);
        //    if (existing == null)
        //        throw new Exception($"GroupStudent record for Group '{groupId}' and Student '{studentId}' does not exist.");

        //    // ===== VALIDATION =====
        //    // Nếu đổi sang Group khác hoặc Student khác => kiểm tra trùng
        //    if ((model.Groupid != 0 && model.Groupid != groupId) ||
        //        (model.Studentid != 0 && model.Studentid != studentId))
        //    {
        //        var exists = await repo.ExistsAsync(model.Groupid, model.Studentid);
        //        if (exists)
        //            throw new Exception($"Student '{model.Studentid}' already belongs to Group '{model.Groupid}'.");
        //    }

        //    // ===== CẬP NHẬT =====
        //    existing.Groupid = model.Groupid != 0 ? model.Groupid : existing.Groupid;
        //    existing.Studentid = model.Studentid != 0 ? model.Studentid : existing.Studentid;
        //    existing.Createat = existing.Createat;
        //    repo.Update(existing);
        //    await UnitOfWork.SaveChangesAsync();

        //    return _mapper.Map<GroupStudentBM>(existing);
        //}

        // ===== GET BY ID =====
        public async Task<GroupStudentBM?> GetByIdAsync(int groupId, int studentId)
        {
            var repo = UnitOfWork.GroupStudentRepository;

            var entity = await repo.GetByIdAsync(groupId, studentId);
            if (entity == null)
                return null;

            return _mapper.Map<GroupStudentBM>(entity);
        }

        // ===== DELETE =====
        public async Task<bool> DeleteAsync(int groupId, int studentId)
        {
            var repo = UnitOfWork.GroupStudentRepository;

            // ===== LẤY DỮ LIỆU GỐC =====
            var existing = await repo.GetByIdAsync(groupId, studentId, trackChanges: true);
            if (existing == null)
                return false;

            // ===== XOÁ =====
            repo.Delete(existing);
            await UnitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
