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
    public class SubjectService : CrudService<Subject, SubjectBM>, ISubjectService
    {
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<Subject>
            GetRepository() => UnitOfWork.SubjectRepository;

        public async Task<PagedResult<SubjectBM>> GetPagedFilteredAsync(SubjectBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.SubjectRepository;

            var repositoryFilter = new Subject
            {
                Subjectid = filter?.Subjectid ?? 0,
                Subjectname = filter?.Subjectname
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<SubjectBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<SubjectBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }
        public IQueryable<SubjectBM> GetODataQueryable()
        {
            var repo = UnitOfWork.SubjectRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<SubjectBM>(query);
        }
        public override async Task<SubjectBM> CreateAsync(SubjectBM model)
        {
            var repo = UnitOfWork.SubjectRepository;

            // Check for unique subject name
            var exists = await repo.ExistsByNameAsync(model.Subjectname, excludeId: 0);
            if (exists)
                throw new ConflictException($"Subject name '{model.Subjectname}' already exists in the system.");

            // Set creation time
            model.Createat = DateTime.UtcNow;

            // Map to entity and add to DB
            var entity = _mapper.Map<Subject>(model);
            await repo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            // Map back to BM and return
            return _mapper.Map<SubjectBM>(entity);
        }

        public override async Task<SubjectBM> UpdateAsync(int id, SubjectBM model)
        {
            var repo = UnitOfWork.SubjectRepository;

            // Get existing record
            var existing = await repo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
                return null;

            // Check for unique subject name (excluding itself)
            var exists = await repo.ExistsByNameAsync(model.Subjectname, excludeId: id);
            if (exists)
                throw new ConflictException($"Subject name '{model.Subjectname}' already exists in the system.");

            // Preserve creation time
            model.Subjectid = id;
            model.Createat = existing.Createat;

            // Map data and save
            _mapper.Map(model, existing);
            repo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<SubjectBM>(existing);
        }

    }
}
