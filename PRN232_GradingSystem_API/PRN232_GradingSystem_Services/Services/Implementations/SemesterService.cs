using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class SemesterService : CrudService<Semester, SemesterBM>, ISemesterService
    {
        private readonly IMapper _mapper;

        public SemesterService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<Semester>
            GetRepository() => UnitOfWork.SemesterRepository;

        public async Task<PagedResult<SemesterBM>> GetPagedFilteredAsync(SemesterBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.SemesterRepository;

            var repositoryFilter = new Semester
            {
                SemesterId = filter?.Semesterid ?? 0,
                SemesterCode = filter?.Semestercode,
                IsActive = filter?.Isactive
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<SemesterBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<SemesterBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public IQueryable<SemesterBM> GetODataQueryable()
        {
            var repo = UnitOfWork.SemesterRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<SemesterBM>(query);
        }

        public override async Task<SemesterBM> UpdateAsync(int id, SemesterBM model)
        {
            var repo = UnitOfWork.SemesterRepository;
            var userRepo = UnitOfWork.UserRepository;

            var exists = await repo.ExistsWithCodeAsync(model.Semestercode, id);
            if (exists)
            {
                throw new ConflictException($"Semester code '{model.Semestercode}' already exists.");
            }
            var userExists = await userRepo.ExistsAsync(model.Updateby.Value);
            if (!userExists)
                throw new NotFoundException($"User with ID '{model.Updateby}' does not exist.");


            var existing = await repo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
            {
                return null;
            }
            model.Semesterid = id;
            model.Createat = existing.CreatedAt;
            model.Createby = existing.CreatedBy;
            _mapper.Map(model, existing);
            repo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<SemesterBM>(existing);
        }
        public override async Task<SemesterBM> CreateAsync(SemesterBM model)
        {
            var repo = UnitOfWork.SemesterRepository;
            var userRepo = UnitOfWork.UserRepository;

            // Kiểm tra nếu code đã tồn tại
            var exists = await repo.ExistsWithCodeAsync(model.Semestercode, excludeId: 0);
            if (exists)
            {
                throw new ConflictException($"Semester code '{model.Semestercode}' already exists.");
            }
            var userExists = await userRepo.ExistsAsync(model.Createby.Value);
            if (!userExists)
                throw new NotFoundException($"User with ID '{model.Createby}' does not exist.");

            // Map sang entity và thêm vào DB
            var entity = _mapper.Map<Semester>(model);
            await repo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            // Map ngược lại để trả kết quả
            return _mapper.Map<SemesterBM>(entity);
        }

        public async Task<SemesterBM?> DeactivateAsync(int id)
        {
            var repo = UnitOfWork.SemesterRepository;

            var existing = await repo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
                return null;

            existing.IsActive = false;
            existing.UpdatedAt = DateTime.UtcNow;

            repo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<SemesterBM>(existing);
        }

    }
}
