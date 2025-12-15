using PRN232_GradingSystem_Repositories.Models;
using PRN232_GradingSystem_Repositories.UnitOfWork;
using PRN232_GradingSystem_Services.BusinessModel;
using PRN232_GradingSystem_Services.Services.Interfaces;

namespace PRN232_GradingSystem_Services.Services.Implementations
{
    public class RubricService : IRubricService
    {
        private readonly IUnitOfWork _uow;

        public RubricService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<ExamRubricBM> GetRubricByExamCodeAsync(string examCode)
        {
            // 1. Lấy Exam theo exam_code
            var exam = await _uow.Repository<Exam>()
                .GetAllAsync(
                    predicate: e => e.ExamCode == examCode,
                    includes: e => e.Questions
                );

            var examEntity = exam.FirstOrDefault();
            if (examEntity == null)
                throw new KeyNotFoundException($"Exam with code '{examCode}' not found");

            // 2. Lấy Question + Criteria
            var questions = await _uow.Repository<Question>()
                .GetAllAsync(
                    predicate: q => q.ExamId == examEntity.ExamId,
                    orderBy: q => q.OrderBy(x => x.QCode),
                    includes: q => q.Criteria
                );

            // 3. Map sang BM
            return new ExamRubricBM
            {
                Examcode = examCode,
                Questions = questions.Select(q => new QuestionRubricBM
                {
                    Questionid = q.QuestionId,
                    Qcode = q.QCode ?? "",
                    Description = q.Description,
                    Maxscore = q.MaxScore ?? 0,
                    Criteria = q.Criteria
                        .OrderBy(c => c.OrderIndex)
                        .Select(c => new CriteriaRubricBM
                        {
                            Criteriaid = c.CriteriaId,
                            Orderindex = c.OrderIndex,
                            Content = c.Content,
                            Weight = c.Weight,
                            Ismanual = c.IsManual ?? true
                        })
                        .ToList()
                }).ToList()
            };
        }
    }
}