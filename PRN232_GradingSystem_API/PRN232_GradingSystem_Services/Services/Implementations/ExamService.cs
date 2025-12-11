using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class ExamService : CrudService<Exam, ExamBM>, IExamService
    {
        private readonly IMapper _mapper;

        public ExamService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<Exam>
            GetRepository() => UnitOfWork.ExamRepository;

        public async Task<PagedResult<ExamBM>> GetPagedFilteredAsync(ExamBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.ExamRepository;

            var repositoryFilter = new Exam
            {
                Examid = filter?.Examid ?? 0,
                Examname = filter?.Examname,
                Semesterid = filter?.Semesterid,
                Subjectid = filter?.Subjectid,
                Examdate = filter?.Examdate,
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<ExamBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<ExamBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public IQueryable<ExamBM> GetODataQueryable()
        {
            var repo = UnitOfWork.ExamRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<ExamBM>(query);
        }

        public async Task<ExamBM?> GetExamByIdAsync(int examId)
        {
            var repo = UnitOfWork.ExamRepository;
            var exam = await repo.GetByIdAsync(examId);
            return exam != null ? _mapper.Map<ExamBM>(exam) : null;
        }
        public override async Task<ExamBM> CreateAsync(ExamBM model)
        {
            var examRepo = UnitOfWork.ExamRepository;
            var semesterRepo = UnitOfWork.SemesterRepository;
            var subjectRepo = UnitOfWork.SubjectRepository;


            var semesterExists = await semesterRepo.ExistsAsync(model.Semesterid.Value);
            if (!semesterExists)
                throw new NotFoundException($"Semester with ID '{model.Semesterid}' does not exist.");

            var subjectExists = await subjectRepo.ExistsAsync(model.Subjectid.Value);
            if (!subjectExists)
                throw new NotFoundException($"Subject with ID '{model.Subjectid}' does not exist.");

            // Kiểm tra trùng tên kỳ thi trong cùng Semester + Subject
            var exists = await examRepo.ExistsByNameAsync(model.Examname, excludeId: 0);
            if (exists)
                throw new ConflictException($"Exam '{model.Examname}' already exists.");

            // === MAP & SAVE ===
            var entity = _mapper.Map<Exam>(model);
            entity.Createat = DateTime.UtcNow;

            await examRepo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<ExamBM>(entity);
        }


        // ===== UPDATE =====
        public override async Task<ExamBM> UpdateAsync(int id, ExamBM model)
        {
            var examRepo = UnitOfWork.ExamRepository;
            var semesterRepo = UnitOfWork.SemesterRepository;
            var subjectRepo = UnitOfWork.SubjectRepository;

            var existing = await examRepo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
                throw new Exception($"Exam with ID '{id}' does not exist.");

            // Nếu có cập nhật Semester hoặc Subject thì kiểm tra tồn tại
            if (model.Semesterid.HasValue)
            {
                var semesterExists = await semesterRepo.ExistsAsync(model.Semesterid.Value);
                if (!semesterExists)
                    throw new NotFoundException($"Semester with ID '{model.Semesterid}' does not exist.");
            }

            if (model.Subjectid.HasValue)
            {
                var subjectExists = await subjectRepo.ExistsAsync(model.Subjectid.Value);
                if (!subjectExists)
                    throw new NotFoundException($"Subject with ID '{model.Subjectid}' does not exist.");
            }

            // Kiểm tra trùng tên kỳ thi trong cùng Semester + Subject
            var exists = await examRepo.ExistsByNameAsync(model.Examname ?? existing.Examname, excludeId: id);
            if (exists)
                throw new ConflictException($"Exam '{model.Examname}' already exists.");

            // === UPDATE FIELDS ===
            existing.Examname = model.Examname ?? existing.Examname;
            existing.Semesterid = model.Semesterid ?? existing.Semesterid;
            existing.Subjectid = model.Subjectid ?? existing.Subjectid;
            existing.Examdate = model.Examdate ?? existing.Examdate;
            existing.Createat = existing.Createat;
            examRepo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<ExamBM>(existing);
        }

    }
}
