using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class GroupService : CrudService<Group, GroupBM>, IGroupService
    {
        private readonly IMapper _mapper;

        public GroupService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<Group>
            GetRepository() => UnitOfWork.GroupRepository;

        public async Task<PagedResult<GroupBM>> GetPagedFilteredAsync(GroupBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.GroupRepository;

            var repositoryFilter = new Group
            {
                Groupid = filter?.Groupid ?? 0,
                Groupname = filter?.Groupname,
                Semesterid = filter?.Semesterid,
                Createby = filter?.Createby,
                Updateby = filter?.Updateby
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<GroupBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<GroupBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }
        public IQueryable<GroupBM> GetODataQueryable()
        {
            var repo = UnitOfWork.GroupRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<GroupBM>(query);
        }
        public override async Task<GroupBM> CreateAsync(GroupBM model)
        {
            var repo = UnitOfWork.GroupRepository;
            var semesterRepo = UnitOfWork.SemesterRepository;
            var userRepo = UnitOfWork.UserRepository;
            // Kiểm tra người tạo tồn tại
            var userExists = await userRepo.ExistsAsync(model.Createby.Value);
            if (!userExists)
                throw new NotFoundException($"User with ID '{model.Createby}' does not exist.");

            // Kiểm tra Semester tồn tại
            var semesterExists = await semesterRepo.ExistsAsync(model.Semesterid.Value);
            if (!semesterExists)
                throw new NotFoundException($"Semester with ID '{model.Semesterid}' does not exist.");

            // Kiểm tra trùng tên nhóm
            var exists = await repo.ExistsWithNameAsync(model.Groupname, model.Semesterid.Value, excludeId: 0);
            if (exists)
                throw new ConflictException($"Group name '{model.Groupname}' already exists in this semester.");

            // ===== MAP & SAVE =====
            var entity = _mapper.Map<Group>(model);
            entity.Createat = DateTime.UtcNow;
            await repo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            // ===== MAP NGƯỢC LẠI =====
            return _mapper.Map<GroupBM>(entity);
        }

        public override async Task<GroupBM> UpdateAsync(int id, GroupBM model)
        {
            var repo = UnitOfWork.GroupRepository;
            var semesterRepo = UnitOfWork.SemesterRepository;
            var userRepo = UnitOfWork.UserRepository;

            // ===== LẤY DỮ LIỆU GỐC =====
            var existing = await repo.GetByIdAsync(id);
            if (existing == null)
                throw new NotFoundException($"Group with ID '{id}' does not exist.");

            // Kiểm tra Semester tồn tại (nếu có)
            if (model.Semesterid.HasValue)
            {
                var semesterExists = await semesterRepo.ExistsAsync(model.Semesterid.Value);
                if (!semesterExists)
                    throw new NotFoundException($"Semester with ID '{model.Semesterid}' does not exist.");
            }
            // Kiểm tra người cập nhật tồn tại
            if (model.Updateby.HasValue)
            {
                var userExists = await userRepo.ExistsAsync(model.Updateby.Value);
                if (!userExists)
                    throw new NotFoundException($"User with ID '{model.Updateby}' does not exist.");
            }

            // Kiểm tra trùng tên nhóm
            var exists = await repo.ExistsWithNameAsync(model.Groupname, model.Semesterid.Value, excludeId: id);
            if (exists)
                throw new ConflictException($"Group name '{model.Groupname}' already exists in this semester.");

           

            // ===== CẬP NHẬT GIÁ TRỊ =====
            existing.Groupname = model.Groupname ?? existing.Groupname;
            existing.Semesterid = model.Semesterid ?? existing.Semesterid;
            existing.Updateby = model.Updateby ?? existing.Updateby;
            existing.Updateat = DateTime.UtcNow;

            repo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            // ===== MAP NGƯỢC LẠI =====
            return _mapper.Map<GroupBM>(existing);
        }

    }
}
