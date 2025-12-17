using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Common;
using PRN232_GradingSystem_Services.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using ValidationException = PRN232_GradingSystem_Services.Common.ValidationException;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class GradeService : CrudService<Grade, GradeBM>, IGradeService
    {
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public GradeService(
            IUnitOfWork unitOfWork, 
            IMapper mapper,
            INotificationService notificationService)
            : base(unitOfWork, mapper)
        {
            _mapper = mapper;
            _notificationService = notificationService;
        }

        protected override PRN232_GradingSystem_Repositories.Repositories.Interfaces.IEntityRepository<Grade>
            GetRepository() => UnitOfWork.GradeRepository;

        public async Task<PagedResult<GradeBM>> GetPagedFilteredAsync(GradeBM filter, int pageNumber, int pageSize)
        {
            var repo = UnitOfWork.GradeRepository;

            var repositoryFilter = new Grade
            {
                GradeId = filter?.Gradeid ?? 0,        
                SubmissionId = filter?.Submissionid,
                Marker = filter?.MarkerNavigation,
                Q1 = filter?.Q1,
                Q2 = filter?.Q2,
                Q3 = filter?.Q3,
                Q4 = filter?.Q4,
                Q5 = filter?.Q5,
                Q6 = filter?.Q6,
                TotalScore = filter?.Totalscore,
                Status = filter?.Status,
                CreatedAt = filter?.Createat,
                UpdatedAt = filter?.Updateat
            };


            var (entities, total) = await repo.GetPagedWithDetailsAsync(repositoryFilter, pageNumber, pageSize);

            var items = _mapper.Map<IReadOnlyList<GradeBM>>(entities);
            var totalPages = (int)Math.Ceiling((double)total / pageSize);

            return new PagedResult<GradeBM>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = total,
                TotalPages = totalPages,
                Items = items
            };
        }

        public IQueryable<GradeBM> GetODataQueryable()
        {
            var repo = UnitOfWork.GradeRepository;
            var query = repo.GetAllWithDetails();

            return _mapper.ProjectTo<GradeBM>(query);
        }


        public override async Task<GradeBM> CreateAsync(GradeBM model)
        {
            var gradeRepo = UnitOfWork.GradeRepository;
            var submissionRepo = UnitOfWork.SubmissionRepository;
            var userRepo = UnitOfWork.UserRepository;

            // ===== VALIDATION =====
            if (!model.Submissionid.HasValue)
                throw new ValidationException("Submission ID must be provided.");

            if (!model.Marker.HasValue)
                throw new ValidationException("Marker (User) must be provided.");

            var submissionExists = await submissionRepo.ExistsAsync(model.Submissionid.Value);
            if (!submissionExists)
                throw new NotFoundException($"Submission with ID '{model.Submissionid}' does not exist.");

            var markerExists = await userRepo.ExistsAsync(model.Marker.Value);
            if (!markerExists)
                throw new NotFoundException($"Marker (User) with ID '{model.Marker}' does not exist.");

            // Quy định thang điểm tối đa cho từng câu
            var maxScores = new Dictionary<string, decimal>
            {
                { "Q1", 1.0m }, { "Q2", 1.5m }, { "Q3", 2.5m },
                { "Q4", 2.0m }, { "Q5", 1.5m }, { "Q6", 1.5m }
            };

            var scoreMap = new Dictionary<string, decimal?>
                {
                    { "Q1", model.Q1 }, { "Q2", model.Q2 }, { "Q3", model.Q3 },
                    { "Q4", model.Q4 }, { "Q5", model.Q5 }, { "Q6", model.Q6 }
                };

            // Phải có ít nhất 1 câu
            if (scoreMap.All(x => x.Value == null))
                throw new ValidationException("At least one question score (Q1–Q6) must be provided.");

            // Kiểm tra hợp lệ
            foreach (var (key, value) in scoreMap)
            {
                if (value.HasValue)
                {
                    if (value < 0)
                        throw new ValidationException($"{key} cannot be negative.");
                    if (value > maxScores[key])
                        throw new ValidationException($"{key} exceeds the maximum allowed score of {maxScores[key]}.");
                }
            }

            model.Totalscore = scoreMap.Values.Where(v => v.HasValue).Sum(v => v.Value);

            var entity = _mapper.Map<Grade>(model);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = null;
            entity.Status = "TeacherVerified";
            await gradeRepo.AddAsync(entity);
            await UnitOfWork.SaveChangesAsync();

            // Gửi SignalR notification
            if (_notificationService != null)
            {
                await _notificationService.SendGradeCreatedNotificationAsync(entity);
            }

            return _mapper.Map<GradeBM>(entity);
        }


        public override async Task<GradeBM> UpdateAsync(int id, GradeBM model)
        {
            var gradeRepo = UnitOfWork.GradeRepository;
            var submissionRepo = UnitOfWork.SubmissionRepository;
            var userRepo = UnitOfWork.UserRepository;

            var existing = await gradeRepo.GetByIdAsync(id, trackChanges: true);
            if (existing == null)
                throw new NotFoundException($"Grade with ID '{id}' does not exist.");

            if (!model.Submissionid.HasValue)
                throw new ValidationException("Submission ID must be provided.");

            if (!model.Marker.HasValue)
                throw new ValidationException("Marker (User) must be provided.");

            var submissionExists = await submissionRepo.ExistsAsync(model.Submissionid.Value);
            if (!submissionExists)
                throw new NotFoundException($"Submission with ID '{model.Submissionid}' does not exist.");

            var markerExists = await userRepo.ExistsAsync(model.Marker.Value);
            if (!markerExists)
                throw new NotFoundException($"Marker (User) with ID '{model.Marker}' does not exist.");

            var maxScores = new Dictionary<string, decimal>
            {
                { "Q1", 1.0m }, { "Q2", 1.5m }, { "Q3", 2.5m },
                { "Q4", 2.0m }, { "Q5", 1.5m }, { "Q6", 1.5m }
            };

            var scoreMap = new Dictionary<string, decimal?>
            {
                { "Q1", model.Q1 ?? existing.Q1 },
                { "Q2", model.Q2 ?? existing.Q2 },
                { "Q3", model.Q3 ?? existing.Q3 },
                { "Q4", model.Q4 ?? existing.Q4 },
                { "Q5", model.Q5 ?? existing.Q5 },
                { "Q6", model.Q6 ?? existing.Q6 }
            };

            foreach (var (key, value) in scoreMap)
            {
                if (value.HasValue)
                {
                    if (value < 0)
                        throw new ValidationException($"{key} cannot be negative.");
                    if (value > maxScores[key])
                        throw new ValidationException($"{key} exceeds the maximum allowed score of {maxScores[key]}.");
                }
            }

            existing.Q1 = scoreMap["Q1"];
            existing.Q2 = scoreMap["Q2"];
            existing.Q3 = scoreMap["Q3"];
            existing.Q4 = scoreMap["Q4"];
            existing.Q5 = scoreMap["Q5"];
            existing.Q6 = scoreMap["Q6"];
            existing.TotalScore = scoreMap.Values.Where(v => v.HasValue).Sum(v => v.Value);
            existing.UpdatedAt = DateTime.UtcNow;

            gradeRepo.Update(existing);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<GradeBM>(existing);
        }

        public async Task<GradeBM> CreateGradeWithDetailsAsync(GradeWithDetailsRequestBM model)
        {
            // ===== VALIDATION GRADE =====
            if (!model.Submissionid.HasValue)
                throw new ValidationException("Submission ID must be provided.");

            if (string.IsNullOrWhiteSpace(model.Marker))
                throw new ValidationException("Marker (email) must be provided.");

            var submissionExists = await UnitOfWork.SubmissionRepository.ExistsAsync(model.Submissionid.Value);
            if (!submissionExists)
                throw new NotFoundException($"Submission with ID '{model.Submissionid}' does not exist.");
            // ===== UPDATE SUBMISSION COMMENT IF PROVIDED =====
            if (!string.IsNullOrWhiteSpace(model.Comment))
            {
                var submission = await UnitOfWork.SubmissionRepository.GetByIdAsync(model.Submissionid.Value);
                if (submission != null)
                {
                    submission.Comment = model.Comment;
                    submission.UpdatedAt = DateTime.UtcNow;

                    UnitOfWork.SubmissionRepository.Update(submission);
                }
            }
            // Kiểm tra email tồn tại
            var markerUserId = await UnitOfWork.UserRepository.GetUserIdByEmailAsync(model.Marker);

            if (markerUserId == null)
                throw new NotFoundException($"Marker user with email '{model.Marker}' does not exist.");

            // Gán lại UserId để xử lý tiếp
            int markerId = markerUserId.Value;

            // ===== VALIDATE GRADEDETAILS =====
            if (model.Gradedetails == null || !model.Gradedetails.Any())
                throw new ValidationException("At least one gradedetail must be provided.");

            var totalSubcodes = MaxScoresData.MaxScores.Sum(q => q.SubScores.Count);
            if (model.Gradedetails.Count > totalSubcodes)
                throw new ValidationException($"Cannot have more than {totalSubcodes} gradedetails.");

            var qsubSet = new HashSet<string>();
            foreach (var gd in model.Gradedetails)
            {
                ValidateSubcodeAndPoint(gd.Qcode, gd.Subcode, gd.Point);

                var key = $"{gd.Qcode}-{gd.Subcode}";
                if (!qsubSet.Add(key))
                    throw new ValidationException($"Duplicate Qcode/Subcode: {key}");
            }

            // ===== CREATE GRADE =====
            var grade = new GradeBM
            {
                Submissionid = model.Submissionid,
                Marker = markerId,
                Q1 = model.Gradedetails.Where(x => x.Qcode == "Q1").Sum(x => x.Point ?? 0),
                Q2 = model.Gradedetails.Where(x => x.Qcode == "Q2").Sum(x => x.Point ?? 0),
                Q3 = model.Gradedetails.Where(x => x.Qcode == "Q3").Sum(x => x.Point ?? 0),
                Q4 = model.Gradedetails.Where(x => x.Qcode == "Q4").Sum(x => x.Point ?? 0),
                Q5 = model.Gradedetails.Where(x => x.Qcode == "Q5").Sum(x => x.Point ?? 0),
                Q6 = model.Gradedetails.Where(x => x.Qcode == "Q6").Sum(x => x.Point ?? 0),
                // Lưu ý: API grading (Qcode = "API") không được tính vào Q1-Q6,
                // nhưng vẫn được tính vào Totalscore
                Totalscore = model.Gradedetails.Sum(x => x.Point ?? 0)
            };

            var gradeEntity = _mapper.Map<Grade>(grade);
            gradeEntity.CreatedAt = DateTime.UtcNow;
            gradeEntity.UpdatedAt = null;
            gradeEntity.Status = "AutoGraded";
            await UnitOfWork.GradeRepository.AddAsync(gradeEntity);
            await UnitOfWork.SaveChangesAsync();

            // ===== CREATE GRADEDETAILS using AutoMapper =====
            foreach (var gdRequest in model.Gradedetails)
            {
                // Map trực tiếp từ GradedetailCreateRequest → GradedetailBM → Gradedetail
                var gdBM = _mapper.Map<GradedetailBM>(gdRequest);
                gdBM.Gradeid = gradeEntity.GradeId; // set Gradeid sau khi grade đã được lưu

                var entity = _mapper.Map<GradeDetail>(gdBM);
                entity.CreatedAt = DateTime.UtcNow;
                entity.UpdatedAt = null;

                await UnitOfWork.GradedetailRepository.AddAsync(entity);
            }

            await UnitOfWork.SaveChangesAsync();

            // Gửi SignalR notification
            if (_notificationService != null)
            {
                await _notificationService.SendGradeCreatedNotificationAsync(gradeEntity);
            }

            return _mapper.Map<GradeBM>(gradeEntity);
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
                throw new ValidationException($"Point for {qcode}.{subcode} must not be bigger than {maxPoint}.");
        }
        public async Task<GradeBM> UpdateStatusAsync(int gradeId, string? status)
        {
            // ===== VALIDATE STATUS =====
            var allowedStatuses = new HashSet<string>
            {
               "AutoGraded",
               "TeacherVerified",
               "ModeratorApproved",
                "ModeratorRejected",
               "AdminApproved",
               "AdminRejected"
            };

            if (string.IsNullOrWhiteSpace(status) || !allowedStatuses.Contains(status))
                throw new ValidationException(
                    $"Invalid status '{status}'. Allowed: {string.Join(", ", allowedStatuses)}");

            // ===== GET GRADE =====
            var grade = await UnitOfWork.GradeRepository.GetByIdAsync(gradeId, trackChanges: true);
            if (grade == null)
                throw new NotFoundException($"Grade with ID {gradeId} not found.");

            // ===== UPDATE STATUS =====
            grade.Status = status;
            grade.UpdatedAt = DateTime.UtcNow;

            UnitOfWork.GradeRepository.Update(grade);
            await UnitOfWork.SaveChangesAsync();

            return _mapper.Map<GradeBM>(grade);
        }

        // Moderator Review Method
        public async Task<GradeBM> ModeratorReviewAsync(
            int gradeId,
            decimal? q1, decimal? q2, decimal? q3, decimal? q4, decimal? q5, decimal? q6,
            string note,
            int moderatorId)
        {
            var grade = await UnitOfWork.GradeRepository.GetByIdAsync(gradeId);
            if (grade == null)
                throw new NotFoundException($"Grade with ID {gradeId} not found.");

            if (q1.HasValue) grade.Q1 = q1.Value;
            if (q2.HasValue) grade.Q2 = q2.Value;
            if (q3.HasValue) grade.Q3 = q3.Value;
            if (q4.HasValue) grade.Q4 = q4.Value;
            if (q5.HasValue) grade.Q5 = q5.Value;
            if (q6.HasValue) grade.Q6 = q6.Value;

            grade.TotalScore = (grade.Q1 ?? 0) + (grade.Q2 ?? 0) + (grade.Q3 ?? 0) +
                               (grade.Q4 ?? 0) + (grade.Q5 ?? 0) + (grade.Q6 ?? 0);

            grade.Status = "ModeratorApproved";
            grade.GradeCount = 2;   
            grade.MarkerId = moderatorId; 
            grade.UpdatedAt = DateTime.UtcNow;

            var moderatorNoteDetail = new GradeDetail
            {
                GradeId = gradeId,
                QCode = "MOD",         
                SubCode = "REVIEW",      
                Point = grade.TotalScore,
                Note = note,            
                CreatedAt = DateTime.UtcNow
            };

            UnitOfWork.GradeRepository.Update(grade);
            await UnitOfWork.GradedetailRepository.AddAsync(moderatorNoteDetail);

            await UnitOfWork.SaveChangesAsync();

            var updatedGrade = await UnitOfWork.GradeRepository.GetByIdWithDetailsAsync(gradeId);
            return _mapper.Map<GradeBM>(updatedGrade);
        }

        // SINH VIÊN GỬI YÊU CẦU PHÚC KHẢO
        public async Task<bool> StudentRequestAppealAsync(int gradeId, string reason, int studentId)
        {
            var grade = await UnitOfWork.GradeRepository.GetByIdWithDetailsAsync(gradeId);

            if (grade == null)
                throw new NotFoundException($"Grade with ID {gradeId} not found.");

            if (grade.Submission == null || grade.Submission.StudentId != studentId)
            {
                throw new ValidationException("You are not authorized to appeal this grade.");
            }

            if (grade.GradeCount >= 2 || grade.Status == "ModeratorApproved")
            {
                throw new ValidationException("This grade has already been reviewed/finalized. Cannot appeal again.");
            }

            if (grade.Status != "TeacherVerified")
            {
                throw new ValidationException("Grade is not ready for appeal (Must be verified by Teacher first).");
            }

            if (grade.Status == "AppealRequested")
            {
                throw new ValidationException("You have already requested an appeal for this grade.");
            }

            grade.Status = "AppealRequested"; 
            grade.UpdatedAt = DateTime.UtcNow;

            var appealNote = new GradeDetail
            {
                GradeId = gradeId,
                QCode = "STUDENT",    
                SubCode = "APPEAL_REASON",
                Point = 0,             
                Note = reason,          
                CreatedAt = DateTime.UtcNow
            };

            UnitOfWork.GradeRepository.Update(grade);
            await UnitOfWork.GradedetailRepository.AddAsync(appealNote);

            return (await UnitOfWork.SaveChangesAsync()) > 0;
        }

        //MODERATOR LẤY DANH SÁCH ĐƠN CHỜ DUYỆT
        public async Task<PagedResult<GradeBM>> GetPendingAppealsAsync(int pageNumber, int pageSize)
        {
            var filter = new GradeBM
            {
                Status = "AppealRequested"
            };

            return await GetPagedFilteredAsync(filter, pageNumber, pageSize);
        }
    }
}
