using AutoMapper;
using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations;

public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        
        public QuestionService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<QuestionBM> CreateAsync(QuestionBM bm)
        {
            // Check exam tồn tại
            var examCount = await _uow.ExamRepository.CountAsync(x => x.ExamId == bm.Examid);
            if (examCount == 0)
                throw new Exception("Exam not found");

            // Check QCode trùng trong exam
            var existed = await _uow.Repository<Question>()
                .CountAsync(x => x.ExamId == bm.Examid && x.QCode == bm.Qcode);

            if (existed > 0)
                throw new Exception("QCode already exists in this exam");

            var entity = new Question
            {
                ExamId = bm.Examid,
                QCode = bm.Qcode,
                Description = bm.Description,
                MaxScore = bm.Maxscore
            };

            await _uow.Repository<Question>().AddAsync(entity);
            await _uow.SaveChangesAsync();

            bm.Questionid = entity.QuestionId;
            return bm;
        }

        public async Task<List<QuestionBM>> GetByExamAsync(int examId)
        {
            var list = await _uow.Repository<Question>()
                .GetAllAsync(
                    predicate: x => x.ExamId == examId,
                    orderBy: q => q.OrderBy(x => x.QCode)
                );

            return list.Select(x => new QuestionBM
            {
                Questionid = x.QuestionId,
                Examid = x.ExamId ?? 0,
                Qcode = x.QCode ?? "",
                Description = x.Description,
                Maxscore = x.MaxScore ?? 0
            }).ToList();
        }

        public async Task<bool> UpdateAsync(int questionId, QuestionBM bm)
        {
            var repo = _uow.Repository<Question>();
            var entity = await repo.GetByIdAsync(questionId, trackChanges: true);

            if (entity == null) return false;

            entity.QCode = bm.Qcode;
            entity.Description = bm.Description;
            entity.MaxScore = bm.Maxscore;

            repo.Update(entity);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int questionId)
        {
            var repo = _uow.Repository<Question>();
            var entity = await repo.GetByIdAsync(questionId, trackChanges: true);

            if (entity == null) return false;

            repo.Delete(entity);
            await _uow.SaveChangesAsync();
            return true;
        }
        
        public async Task<QuestionBM> GetByIdAsync(int questionId)
        {
            var entity = await _uow.Repository<Question>()
                .GetByIdAsync(questionId);

            if (entity == null)
                return null;

            return _mapper.Map<QuestionBM>(entity);
        }
    }