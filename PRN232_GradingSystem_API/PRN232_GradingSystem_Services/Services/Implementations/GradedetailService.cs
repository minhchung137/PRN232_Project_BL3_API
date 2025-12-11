using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class GradedetailService : CrudService<GradeDetail, GradedetailBM>, IGradedetailService
    {
        private readonly IMapper _mapper;

        public GradedetailService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<GradeDetail>
            GetRepository() => UnitOfWork.GradedetailRepository;

        public async Task<PagedResult<GradedetailBM>> GetPagedFilteredAsync(GradedetailBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.GradedetailRepository;

            var repositoryFilter = new GradeDetail
            {
                GradeDetailId = filter?.Gradedetailid ?? 0,
                GradeId = filter?.Gradeid,
                QCode = filter?.Qcode,
                SubCode = filter?.Subcode
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<GradedetailBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<GradedetailBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public IQueryable<GradedetailBM> GetODataQueryable()
        {
            var repo = UnitOfWork.GradedetailRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<GradedetailBM>(query);
        }
        public override async Task<GradedetailBM> CreateAsync(GradedetailBM model)
        {
            var gradedetailRepo = UnitOfWork.GradedetailRepository;
            var gradeRepo = UnitOfWork.GradeRepository;

            // ===== VALIDATION GRADE =====
            if (!model.Gradeid.HasValue)
                throw new ValidationException("Gradeid must be provided.");

            var gradeExists = await gradeRepo.ExistsAsync(model.Gradeid.Value);
            if (!gradeExists)
                throw new NotFoundException($"Grade with ID '{model.Gradeid}' does not exist.");

            // ===== VALIDATE SUBCODE & POINT =====
            ValidateSubcodeAndPoint(model.Qcode, model.Subcode, model.Point);

            // ===== CHECK TRÙNG VỚI GRADEID =====
            var existingDetails = await gradedetailRepo.GetAllAsync(d => d.GradeId == model.Gradeid.Value);

            // 1. Kiểm tra số lượng tối đa (tổng subcode)
            int totalSubcodes = MaxScoresData.MaxScores.Sum(q => q.SubScores.Count);
            if (existingDetails.Count >= totalSubcodes)
                throw new ValidationException($"Gradeid '{model.Gradeid}' already has maximum number of gradedetails ({totalSubcodes}).");

            // 2. Kiểm tra trùng Qcode + Subcode
            bool isDuplicate = existingDetails.Any(d => d.QCode == model.Qcode && d.SubCode == model.Subcode);
            if (isDuplicate)
                throw new ValidationException($"Gradedetail with Qcode '{model.Qcode}' and Subcode '{model.Subcode}' already exists for Gradeid '{model.Gradeid}'.");

            // ===== MAP & SAVE =====
            var entity = _mapper.Map<GradeDetail>(model);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = null;

            await gradedetailRepo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<GradedetailBM>(entity);
        }

        public override async Task<GradedetailBM> UpdateAsync(int id, GradedetailBM model)
        {
            var gradedetailRepo = UnitOfWork.GradedetailRepository;
            var gradeRepo = UnitOfWork.GradeRepository;

            var existing = await gradedetailRepo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
                throw new NotFoundException($"Gradedetail with ID '{id}' does not exist.");

            // ===== VALIDATE POINT =====
            ValidateSubcodeAndPoint(existing.QCode, existing.SubCode, model.Point);

            // ===== UPDATE GRADEDETAIL =====
            if (model.Point.HasValue)
                existing.Point = model.Point;

            if (!string.IsNullOrWhiteSpace(model.Note))
                existing.Note = model.Note;

            existing.UpdatedAt = DateTime.UtcNow;

            gradedetailRepo.Update(existing);

            // ===== RELOAD ALL DETAILS OF THIS GRADE =====
            var allDetails = await gradedetailRepo.GetByGradeIdAsync(existing.GradeId.Value);

            // ===== RECALCULATE GRADE =====
            var grade = await gradeRepo.GetByIdAsync(existing.GradeId.Value, trackChanges: true);
            if (grade == null)
                throw new NotFoundException($"Grade with ID '{existing.GradeId}' not found.");

            grade.Q1 = allDetails.Where(x => x.QCode == "Q1").Sum(x => x.Point ?? 0);
            grade.Q2 = allDetails.Where(x => x.QCode == "Q2").Sum(x => x.Point ?? 0);
            grade.Q3 = allDetails.Where(x => x.QCode == "Q3").Sum(x => x.Point ?? 0);
            grade.Q4 = allDetails.Where(x => x.QCode == "Q4").Sum(x => x.Point ?? 0);
            grade.Q5 = allDetails.Where(x => x.QCode == "Q5").Sum(x => x.Point ?? 0);
            grade.Q6 = allDetails.Where(x => x.QCode == "Q6").Sum(x => x.Point ?? 0);

            grade.TotalScore = allDetails.Sum(x => x.Point ?? 0);
            grade.UpdatedAt = DateTime.UtcNow;

            gradeRepo.Update(grade);

            // ===== SAVE ALL =====
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<GradedetailBM>(existing);
        }


        private void ValidateSubcodeAndPoint(string qcode, string subcode, decimal? point)
        {
            if (string.IsNullOrWhiteSpace(subcode))
                throw new ValidationException("Subcode must be provided.");

            var question = MaxScoresData.MaxScores.FirstOrDefault(q => q.Qcode == qcode);
            if (question == null)
                throw new ValidationException($"Qcode '{qcode}' is not allowed.");

            if (!question.SubScores.ContainsKey(subcode))
                throw new ValidationException($"Subcode '{subcode}' is not valid for Qcode '{qcode}'.");

            var maxPoint = question.SubScores[subcode];
            if (point.HasValue && point > maxPoint)
                throw new ValidationException($"Point for {qcode}.{subcode} must be smaller than {maxPoint}.");
        }
        public async Task UpdateManyAsync(GradedetailUpdateRequestListBM request)
        {
            if (request.Gradedetails == null || !request.Gradedetails.Any())
                throw new ValidationException("At least one gradedetail must be provided.");

            var gradedetailRepo = UnitOfWork.GradedetailRepository;
            var gradeRepo = UnitOfWork.GradeRepository;

            var gradeIds = new List<int>();

            foreach (var item in request.Gradedetails)
            {
                var existing = await gradedetailRepo.GetByIdAsync(item.Gradedetailid, trackChanges: true);
                if (existing == null)
                    throw new NotFoundException($"Gradedetail with ID '{item.Gradedetailid}' not found.");

                // Validate point
                ValidateSubcodeAndPoint(existing.QCode, existing.SubCode, item.Point);

                if (item.Point.HasValue)
                    existing.Point = item.Point;

                if (!string.IsNullOrWhiteSpace(item.Note))
                    existing.Note = item.Note;

                existing.UpdatedAt = DateTime.UtcNow;
                gradedetailRepo.Update(existing);

                if (!existing.GradeId.HasValue)
                    throw new ValidationException($"Gradedetail {existing.GradeDetailId} has no GradeId assigned.");

                gradeIds.Add(existing.GradeId.Value);
            }

            var distinctGradeIds = gradeIds.Distinct();

            foreach (var gid in distinctGradeIds)
            {
                var allDetails = await gradedetailRepo.GetByGradeIdAsync(gid);

                var grade = await gradeRepo.GetByIdAsync(gid, trackChanges: true);
                if (grade == null)
                    throw new NotFoundException($"Grade with ID '{gid}' not found.");

                grade.Q1 = allDetails.Where(x => x.QCode == "Q1").Sum(x => x.Point ?? 0);
                grade.Q2 = allDetails.Where(x => x.QCode == "Q2").Sum(x => x.Point ?? 0);
                grade.Q3 = allDetails.Where(x => x.QCode == "Q3").Sum(x => x.Point ?? 0);
                grade.Q4 = allDetails.Where(x => x.QCode == "Q4").Sum(x => x.Point ?? 0);
                grade.Q5 = allDetails.Where(x => x.QCode == "Q5").Sum(x => x.Point ?? 0);
                grade.Q6 = allDetails.Where(x => x.QCode == "Q6").Sum(x => x.Point ?? 0);
                grade.TotalScore = allDetails.Sum(x => x.Point ?? 0);

                grade.UpdatedAt = DateTime.UtcNow;
                gradeRepo.Update(grade);
            }

            await UnitOfWork.SaveChangesAsync();
        }
    }
}
