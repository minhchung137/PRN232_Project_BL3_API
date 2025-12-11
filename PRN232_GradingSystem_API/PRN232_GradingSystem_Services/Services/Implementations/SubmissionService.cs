using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class SubmissionService : CrudService<Submission, SubmissionBM>, ISubmissionService
    {
        private readonly IMapper _mapper;

        public SubmissionService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<Submission>
            GetRepository() => UnitOfWork.SubmissionRepository;

        public async Task<PagedResult<SubmissionBM>> GetPagedFilteredAsync(SubmissionBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.SubmissionRepository;

            var repositoryFilter = new Submission
            {
                SubmissionId = filter?.Submissionid ?? 0,
                ExamId = filter?.Examid,
                StudentId = filter?.Studentid
            };

            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<SubmissionBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<SubmissionBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public IQueryable<SubmissionBM> GetODataQueryable()
        {
            var repo = UnitOfWork.SubmissionRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<SubmissionBM>(query);
        }

        public async Task<SubmissionBM> FindOrCreateSubmissionAsync(int examId, int studentId, string fileUrl, string studentRoll = null)
        {
            var repo = UnitOfWork.SubmissionRepository;
            var existing = repo.GetAllWithDetails()
                .FirstOrDefault(s => s.ExamId == examId && s.StudentId == studentId);

            if (existing != null)
            {
                bool needsUpdate = false;
                
                // Update FileUrl if provided
                if (!string.IsNullOrWhiteSpace(fileUrl) && existing.FileUrl != fileUrl)
                {
                    existing.FileUrl = fileUrl;
                    needsUpdate = true;
                }
                
                // Update Solution with studentRoll if provided
                if (!string.IsNullOrWhiteSpace(studentRoll) && existing.Solution != studentRoll)
                {
                    existing.Solution = studentRoll;
                    needsUpdate = true;
                }
                
                if (needsUpdate)
                {
                    existing.UpdatedAt = DateTime.UtcNow;
                    repo.Update(existing);
                    await UnitOfWork.SaveChangesAsync();
                }
                
                return _mapper.Map<SubmissionBM>(existing);
            }

            // Create new submission
            var newSubmission = new SubmissionBM
            {
                Examid = examId,
                Studentid = studentId,
                Fileurl = fileUrl,
                Solution = studentRoll,
                Createat = DateTime.UtcNow
            };

            return await CreateAsync(newSubmission);
        }

        public  async Task<SubmissionBM> CreateSubmissionAsync(SubmissionBM model)
        {
            var submissionRepo = UnitOfWork.SubmissionRepository;
            var examRepo = UnitOfWork.ExamRepository;
            var studentRepo = UnitOfWork.StudentRepository;

            // ===== VALIDATION =====
            if (!model.Examid.HasValue)
                throw new ValidationException("Exam ID must be provided.");

            if (!model.Studentid.HasValue)
                throw new ValidationException("Student ID must be provided.");

            var examExists = await examRepo.ExistsAsync(model.Examid.Value);
            if (!examExists)
                throw new NotFoundException($"Exam with ID '{model.Examid}' does not exist.");

            var studentExists = await studentRepo.ExistsAsync(model.Studentid.Value);
            if (!studentExists)
                throw new NotFoundException($"Student with ID '{model.Studentid}' does not exist.");

            // Optional: Validate Solution length, Comment length, File URL format, etc.
            if (!string.IsNullOrWhiteSpace(model.Solution) && model.Solution.Length > 5000)
                throw new ValidationException("Solution cannot exceed 5000 characters.");

            if (!string.IsNullOrWhiteSpace(model.Comment) && model.Comment.Length > 1000)
                throw new ValidationException("Comment cannot exceed 1000 characters.");

            var entity = _mapper.Map<Submission>(model);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = null;
            entity.SubmissionId = 0;

            await submissionRepo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<SubmissionBM>(entity);
        }

        public  async Task<SubmissionBM> UpdateSubmissionAsync(int id, SubmissionBM model)
        {
            var submissionRepo = UnitOfWork.SubmissionRepository;
            var examRepo = UnitOfWork.ExamRepository;
            var studentRepo = UnitOfWork.StudentRepository;

            var existing = await submissionRepo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
                throw new NotFoundException($"Submission with ID '{id}' does not exist.");

            if (model.Examid.HasValue)
            {
                var examExists = await examRepo.ExistsAsync(model.Examid.Value);
                if (!examExists)
                    throw new NotFoundException($"Exam with ID '{model.Examid}' does not exist.");
                existing.ExamId = model.Examid;
            }

            if (model.Studentid.HasValue)
            {
                var studentExists = await studentRepo.ExistsAsync(model.Studentid.Value);
                if (!studentExists)
                    throw new NotFoundException($"Student with ID '{model.Studentid}' does not exist.");
                existing.StudentId = model.Studentid;
            }

            if (!string.IsNullOrWhiteSpace(model.Solution))
            {
                if (model.Solution.Length > 5000)
                    throw new ValidationException("Solution cannot exceed 5000 characters.");
                existing.Solution = model.Solution;
            }

            if (!string.IsNullOrWhiteSpace(model.Comment))
            {
                if (model.Comment.Length > 1000)
                    throw new ValidationException("Comment cannot exceed 1000 characters.");
                existing.Comment = model.Comment;
            }

            if (!string.IsNullOrWhiteSpace(model.Fileurl))
                existing.FileUrl = model.Fileurl;

            existing.UpdatedAt = DateTime.UtcNow;

            submissionRepo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<SubmissionBM>(existing);
        }

    }
}
