using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class SemesterSubjectService : CrudService<SemesterSubject, SemesterSubjectBM>, ISemesterSubjectService
    {
        private readonly IMapper _mapper;

        public SemesterSubjectService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<SemesterSubject>
            GetRepository() => UnitOfWork.SemesterSubjectRepository;

        public async Task<PagedResult<SemesterSubjectBM>> GetPagedFilteredAsync(SemesterSubjectBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.SemesterSubjectRepository;

            var repositoryFilter = new SemesterSubject
            {
                Semesterid = filter?.Semesterid ?? 0,
                Subjectid = filter?.Subjectid ?? 0
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<SemesterSubjectBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<SemesterSubjectBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public IQueryable<SemesterSubjectBM> GetODataQueryable()
        {
            var repo = UnitOfWork.SemesterSubjectRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<SemesterSubjectBM>(query);
        }

        public override async Task<SemesterSubjectBM> CreateAsync(SemesterSubjectBM model)
        {
            var repo = UnitOfWork.SemesterSubjectRepository;
            var subjectRepo = UnitOfWork.SubjectRepository;
            var semesterRepo = UnitOfWork.SemesterRepository;

            // Kiểm tra xem Subject có tồn tại không
            var subjectExists = await subjectRepo.ExistsAsync(model.Subjectid);
            if (!subjectExists)
                throw new NotFoundException($"Subject with ID '{model.Subjectid}' does not exist.");

            // Kiểm tra xem Semester có tồn tại không
            var semesterExists = await semesterRepo.ExistsAsync(model.Semesterid);
            if (!semesterExists)
                throw new NotFoundException($"Semester with ID '{model.Semesterid}' does not exist.");

            // Kiểm tra xem môn học đã tồn tại trong kỳ đó chưa
            var exists = await repo.ExistsAsync(model.Semesterid, model.Subjectid);
            if (exists)
                throw new ConflictException($"Subject ID '{model.Subjectid}' already exists in Semester ID '{model.Semesterid}'.");

            // Set thời gian tạo
            model.Createat = DateTime.UtcNow;

            // Map sang entity và thêm vào DB
            var entity = _mapper.Map<SemesterSubject>(model);
            await repo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            // Map ngược lại và trả về
            return _mapper.Map<SemesterSubjectBM>(entity);
        }


        public async Task<bool> DeleteAsync(int semesterId, int subjectId)
        {
            var repo = UnitOfWork.SemesterSubjectRepository;
            var existing = await repo.GetByKeysAsync(semesterId, subjectId, trackChanges: true);
            if (existing == null) return false;

            repo.Delete(existing);
            await UnitOfWork.SaveChangesAsync();
            return true;
        }
       
    }
}
